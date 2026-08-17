namespace LabAnalyzerConnector.Protocols.ASTM.Models;

public sealed class AstmPatientRecord
{
    public string RawRecord { get; }

    public string? PatientId { get; set; }

    public string? PatientName { get; set; }

    public AstmPatientRecord(
        string rawRecord)
    {
        RawRecord = rawRecord;
    }
}