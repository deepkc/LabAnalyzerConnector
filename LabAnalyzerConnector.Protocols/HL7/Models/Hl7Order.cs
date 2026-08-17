namespace LabAnalyzerConnector.Protocols.HL7.Models;

public sealed class Hl7Order
{
    public string? SampleId { get; set; }

    public string? OrderNumber { get; set; }

    public string? TestName { get; set; }

    public string? TestCode { get; set; }

    public DateTime? CollectionDateTime { get; set; }

    public string? AnalyzerName { get; set; }

    public string? ValidationStatus { get; set; }
}