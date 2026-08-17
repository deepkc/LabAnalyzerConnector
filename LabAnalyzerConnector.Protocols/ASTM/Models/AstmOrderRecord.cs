namespace LabAnalyzerConnector.Protocols.ASTM.Models;

public sealed class AstmOrderRecord
{
    public string RawRecord { get; }

    public string? SampleId { get; set; }

    public string? TestCode { get; set; }

    public AstmOrderRecord(
        string rawRecord)
    {
        RawRecord = rawRecord;
    }
}