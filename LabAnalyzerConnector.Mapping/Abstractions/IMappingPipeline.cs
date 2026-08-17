using LabAnalyzerConnector.Mapping.Models;

namespace LabAnalyzerConnector.Mapping.Abstractions;

public interface IMappingPipeline
{
    Task<MappingPipelineResult> ProcessAsync(
        Guid analyzerId,
        IReadOnlyDictionary<string, string?> sourceFields,
        CancellationToken cancellationToken = default);
}