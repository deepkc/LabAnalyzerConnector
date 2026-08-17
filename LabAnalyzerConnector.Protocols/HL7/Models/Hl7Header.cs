namespace LabAnalyzerConnector.Protocols.HL7.Models;

public sealed class Hl7Header
{
    public string? SendingApplication { get; set; }

    public string? SendingFacility { get; set; }

    public string? ReceivingApplication { get; set; }

    public string? ReceivingFacility { get; set; }

    public string? MessageType { get; set; }

    public string? TriggerEvent { get; set; }

    public string? MessageControlId { get; set; }

    public string? Version { get; set; }

    public DateTime? MessageDateTime { get; set; }
}