namespace LabAnalyzerConnector.Protocols.ASTM.Models;

public sealed class AstmMessage
{
    public AstmHeaderRecord? Header { get; set; }

    public AstmPatientRecord? Patient { get; set; }

    public AstmOrderRecord? Order { get; set; }

    public AstmOrderQuery? OrderQuery { get; set; }

    public List<AstmResultRecord> Results { get; } = new();
}