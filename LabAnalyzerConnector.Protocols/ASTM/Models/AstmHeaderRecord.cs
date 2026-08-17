namespace LabAnalyzerConnector.Protocols.ASTM.Models;

public sealed class AstmHeaderRecord
{
    public string RawRecord { get; }

    public string? SenderName { get; set; }

    public string? SenderVersion { get; set; }

    public AstmHeaderRecord(
        string rawRecord)
    {
        RawRecord = rawRecord;
    }
}