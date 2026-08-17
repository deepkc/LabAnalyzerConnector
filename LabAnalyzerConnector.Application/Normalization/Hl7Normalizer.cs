using LabAnalyzerConnector.Domain.Entities;
using LabAnalyzerConnector.Protocols.HL7.Models;
using LabAnalyzerConnector.Protocols.HL7.Filtering;

namespace LabAnalyzerConnector.Application.Normalization;

public sealed class Hl7Normalizer : INormalizer
{

    private readonly Hl7ResultFilter _resultFilter;

    public Hl7Normalizer(Hl7ResultFilter resultFilter)
    {
        _resultFilter = resultFilter;
    }
    public bool CanNormalize(object parsedMessage)
    {
        return parsedMessage is Hl7Message;
    }

    public IEnumerable<NormalizedLabMessage> Normalize(
        Guid analyzerId,
        object parsedMessage)
    {
        if (parsedMessage is not Hl7Message message)
        {
            throw new ArgumentException(
                "Expected an HL7 message.",
                nameof(parsedMessage));
        }

        Console.WriteLine("========================================");
        Console.WriteLine("HL7 NORMALIZER START");
        Console.WriteLine("========================================");

        Console.WriteLine(
            $"Patient Id : {message.Patient?.PatientId}");

        Console.WriteLine(
            $"Sample Id  : {message.Order?.SampleId}");

        Console.WriteLine(
            $"Test Code  : {message.Order?.TestCode}");

        Console.WriteLine(
            $"Observations Found : {message.Observations.Count}");

        yield return NormalizeMessage(
     analyzerId,
     message,
     _resultFilter);
    }

    private NormalizedLabMessage NormalizeMessage(
     Guid analyzerId,
     Hl7Message message,
     Hl7ResultFilter filter)
    {
        NormalizedLabMessage normalized =
            new()
            {
                Id = Guid.NewGuid(),

                AnalyzerId = analyzerId,

                PatientId =
                    message.Patient?.PatientId,

                SampleId =
                    message.Order?.SampleId,

                Barcode =
                    message.Order?.SampleId,

                ReceivedAtUtc =
                    DateTime.UtcNow
            };

        Console.WriteLine("----------------------------------------");
        Console.WriteLine("Creating Lab Results");
        Console.WriteLine("----------------------------------------");

        foreach (Hl7Observation observation in message.Observations)
        {
            if (!filter.IsResultObservation(observation))
                continue;
            Console.WriteLine(
                $"OBX -> {observation.TestCode} = {observation.Value}");

            LabResult result =
                new()
                {
                    Id = Guid.NewGuid(),

                    AnalyzerId =
                        analyzerId,

                    PatientId =
                        normalized.PatientId,

                    SampleId =
                        normalized.SampleId,

                    TestCode =
                        observation.TestCode,

                    ResultValue =
                        observation.Value,

                    Units =
                        observation.Units,

                    ReferenceRange =
                        observation.ReferenceRange,

                    AbnormalFlag =
                        observation.AbnormalFlag,

                    ResultDateTime =
                        DateTime.UtcNow
                };

            normalized.Results.Add(result);

            Console.WriteLine(
                $"Added Result -> {result.TestCode} = {result.ResultValue}");
        }

        Console.WriteLine("----------------------------------------");
        Console.WriteLine(
            $"HL7 Final Result Count = {normalized.Results.Count}");
        Console.WriteLine("----------------------------------------");

        foreach (LabResult result in normalized.Results)
        {
            Console.WriteLine(
                $"{result.TestCode,-10} {result.ResultValue,-10} {result.Units}");
        }

        Console.WriteLine("========================================");
        Console.WriteLine("HL7 NORMALIZER END");
        Console.WriteLine("========================================");

        return normalized;
    }
}