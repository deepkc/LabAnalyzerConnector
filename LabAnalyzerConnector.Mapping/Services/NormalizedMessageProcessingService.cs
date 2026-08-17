using LabAnalyzerConnector.Domain.Entities;
using LabAnalyzerConnector.Mapping.Abstractions;
using LabAnalyzerConnector.Mapping.Models;

namespace LabAnalyzerConnector.Mapping.Services;

public sealed class NormalizedMessageProcessingService
{
    private readonly IMappingPipeline _mappingPipeline;

    public NormalizedMessageProcessingService(
        IMappingPipeline mappingPipeline)
    {
        _mappingPipeline = mappingPipeline;
    }

    public async Task<NormalizedLabMessage> ProcessAsync(
        NormalizedLabMessage message,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(message);

        var processedResults =
            new List<LabResult>();

        foreach (LabResult result in message.Results)
        {
            var sourceFields =
                new Dictionary<string, string?>
                {
                    ["TestCode"] =
                        result.TestCode,

                    ["TestName"] =
                        result.TestName,

                    ["ResultValue"] =
                        result.ResultValue,

                    ["Units"] =
                        result.Units,

                    ["ReferenceRange"] =
                        result.ReferenceRange,

                    ["AbnormalFlag"] =
                        result.AbnormalFlag
                };

            MappingPipelineResult mappingResult =
                await _mappingPipeline.ProcessAsync(
                    message.AnalyzerId,
                    sourceFields,
                    cancellationToken);

            // =====================================================
            // MAPPING FAILED
            // =====================================================

            if (!mappingResult.IsSuccess ||
                mappingResult.Result is null)
            {
                System.Diagnostics.Debug.WriteLine(
                    $"MAPPING FAILED -> {result.TestCode}");

                foreach (string error in mappingResult.Errors)
                {
                    System.Diagnostics.Debug.WriteLine(
                        $"MAPPING ERROR -> {error}");
                }

                // IMPORTANT:
                // Do NOT add the original analyzer result.
                //
                // If there is no mapping in the database,
                // this result must NOT continue toward persistence.

                continue;
            }

            // =====================================================
            // MAPPING SUCCESS
            // =====================================================

            System.Diagnostics.Debug.WriteLine(
                $"MAPPING SUCCESS -> {result.TestCode}");

            LabResult mappedResult =
                mappingResult.Result;

            // =====================================================
            // Preserve analyzer/message identity
            // =====================================================

            mappedResult.AnalyzerId =
                message.AnalyzerId;

            mappedResult.AnalyzerName =
                message.AnalyzerName;

            mappedResult.PatientId =
                message.PatientId;

            mappedResult.SampleId =
                message.SampleId;

            // =====================================================
            // Preserve message audit information
            // =====================================================

            mappedResult.ReceivedAtUtc =
                message.ReceivedAtUtc;

            mappedResult.RawMessage =
                message.RawMessage;

            // =====================================================
            // ONLY mapped results enter processedResults
            // =====================================================

            processedResults.Add(
                mappedResult);
        }

        System.Diagnostics.Debug.WriteLine(
            $"Original Results Count = {message.Results.Count}");

        System.Diagnostics.Debug.WriteLine(
            $"Mapped Results Count = {processedResults.Count}");

        // Replace original results with ONLY successfully
        // mapped results.
        message.Results =
            processedResults;

        return message;
    }
}