namespace LabAnalyzerConnector.Mapping.Models;

public sealed class UnitConversionRule
{
    public Guid Id { get; set; }

    public Guid AnalyzerId { get; set; }

    public string TestCode { get; set; } = string.Empty;

    public string SourceUnit { get; set; } = string.Empty;

    public string TargetUnit { get; set; } = string.Empty;

    public decimal Multiplier { get; set; } = 1m;

    public decimal Offset { get; set; } = 0m;

    public int DecimalPlaces { get; set; } = 2;

    public bool IsActive { get; set; } = true;
}