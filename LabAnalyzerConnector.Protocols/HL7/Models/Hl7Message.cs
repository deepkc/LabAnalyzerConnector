namespace LabAnalyzerConnector.Protocols.HL7.Models;

public sealed class Hl7Message
{
    public Hl7Header? Header { get; set; }

    public Hl7Patient? Patient { get; set; }

    public Hl7Order? Order { get; set; }

    public List<Hl7Observation> Observations { get; } = new();
}