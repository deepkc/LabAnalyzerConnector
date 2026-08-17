namespace LabAnalyzerConnector.Protocols.ASTM.Models;

public sealed class AstmResultRecord
{
    public string RawRecord { get; }

    public string? TestCode { get; set; }

    public string? Value { get; set; }

    public string? Units { get; set; }

    public string? ReferenceRange { get; set; }

    public string? AbnormalFlag { get; set; }

    public AstmResultRecord(
        string rawRecord)
    {
        RawRecord = rawRecord;
    }
}