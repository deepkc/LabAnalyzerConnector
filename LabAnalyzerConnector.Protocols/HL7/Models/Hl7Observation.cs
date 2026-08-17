namespace LabAnalyzerConnector.Protocols.HL7.Models;

public sealed class Hl7Observation
{
    public int SequenceNumber { get; set; }

    public string? LoincCode { get; set; }

    public string? TestCode { get; set; }

    public string? CodingSystem { get; set; }

    public string? Value { get; set; }

    public string? Units { get; set; }

    public string? ReferenceRange { get; set; }

    public string? AbnormalFlag { get; set; }

    public string? ResultStatus { get; set; }
}