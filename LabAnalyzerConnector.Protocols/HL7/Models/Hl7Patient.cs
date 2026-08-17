namespace LabAnalyzerConnector.Protocols.HL7.Models;

public sealed class Hl7Patient
{
    public string? PatientId { get; set; }

    public string? PatientName { get; set; }

    public DateTime? DateOfBirth { get; set; }

    public string? Sex { get; set; }
}