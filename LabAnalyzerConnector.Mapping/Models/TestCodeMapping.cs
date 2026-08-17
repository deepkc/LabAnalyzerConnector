namespace LabAnalyzerConnector.Mapping.Models;

public sealed class TestCodeMapping
{
    public Guid Id { get; set; }

    public Guid AnalyzerId { get; set; }

    public string AnalyzerTestCode { get; set; } = string.Empty;

    public string StandardTestCode { get; set; } = string.Empty;

    public string? StandardTestName { get; set; }

    public string? AnalyzerTestName { get; set; }

    public string? ExpectedUnit { get; set; }

    public string? StandardUnit { get; set; }

    public bool IsActive { get; set; } = true;
}