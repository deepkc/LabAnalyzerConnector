namespace LabAnalyzerConnector.Mapping.Models;

public sealed class FieldMapping
{
    public Guid Id { get; set; }

    public Guid AnalyzerId { get; set; }

    public string MessageType { get; set; } = string.Empty;

    public string SourceField { get; set; } = string.Empty;

    public string TargetField { get; set; } = string.Empty;

    public bool IsRequired { get; set; }

    public bool IsActive { get; set; } = true;
}