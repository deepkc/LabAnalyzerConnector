using LabAnalyzerConnector.Domain.Entities;
using LabAnalyzerConnector.Protocols.ASTM.Models;

namespace LabAnalyzerConnector.Application.Normalization;

public sealed class AstmNormalizer : INormalizer
{
    public bool CanNormalize(
        object parsedMessage)
    {
        return parsedMessage is AstmMessage;
    }

    public IEnumerable<NormalizedLabMessage> Normalize(
        Guid analyzerId,
        object parsedMessage)
    {
        if (parsedMessage is not AstmMessage message)
        {
            throw new ArgumentException(
                "Expected an ASTM message.",
                nameof(parsedMessage));
        }

        yield return NormalizeMessage(
            analyzerId,
            message);
    }

    private static NormalizedLabMessage NormalizeMessage(
        Guid analyzerId,
        AstmMessage message)
    {
        var normalizedMessage =
     new NormalizedLabMessage
     {
         Id = Guid.NewGuid(),

         AnalyzerId =
             analyzerId,

         PatientId =
             message.Patient?.PatientId,

         SampleId =
             message.Order?.SampleId,

         Barcode =
             message.Order?.SampleId,

         ReceivedAtUtc =
             DateTime.UtcNow
     };

        System.Diagnostics.Debug.WriteLine(
    $"AstmNormalizer -> Parser Results = {message.Results.Count}");

        foreach (
            AstmResultRecord astmResult
            in message.Results)
        {
            System.Diagnostics.Debug.WriteLine(
    $"Adding Result: {astmResult.TestCode}");
            var labResult =
     new LabResult
     {


         Id = Guid.NewGuid(),

         AnalyzerId =
             analyzerId,

         PatientId =
             message.Patient?.PatientId,

         SampleId =
             message.Order?.SampleId,

         TestCode =
             astmResult.TestCode,

         ResultValue =
             astmResult.Value,

         Units =
             astmResult.Units,

         ReferenceRange =
             astmResult.ReferenceRange,

         AbnormalFlag =
             astmResult.AbnormalFlag,

         ResultDateTime =
             DateTime.UtcNow
     };

            normalizedMessage.Results.Add(
                labResult);
        }
        System.Diagnostics.Debug.WriteLine(
    $"AstmNormalizer -> Final Results = {normalizedMessage.Results.Count}");
        return normalizedMessage;
    }
}