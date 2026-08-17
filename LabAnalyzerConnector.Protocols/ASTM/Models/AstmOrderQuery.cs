namespace LabAnalyzerConnector.Protocols.ASTM.Models;

public sealed class AstmOrderQuery
{
    public string RawRecord { get; }

    public string? SampleId { get; set; }

    public AstmOrderQuery(
        string rawRecord)
    {
        RawRecord = rawRecord;
    }
}