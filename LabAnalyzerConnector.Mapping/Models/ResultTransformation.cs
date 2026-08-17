namespace LabAnalyzerConnector.Mapping.Models;

public sealed class ResultTransformation
{
    public Guid Id { get; set; }

    public Guid AnalyzerId { get; set; }

    public string TestCode { get; set; } = string.Empty;

    public string SourceValue { get; set; } = string.Empty;

    public string TargetValue { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;
}