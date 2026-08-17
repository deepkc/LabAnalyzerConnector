namespace LabAnalyzerConnector.Mapping.Models;

public sealed class MappedField
{
    public string SourceField { get; set; } = string.Empty;

    public string TargetField { get; set; } = string.Empty;

    public string? Value { get; set; }

    public bool IsRequired { get; set; }
}