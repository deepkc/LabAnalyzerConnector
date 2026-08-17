namespace LabAnalyzerSimulator.Models;

public sealed class SimulatedResult
{
    public string TestCode { get; set; } = string.Empty;

    public string Result { get; set; } = string.Empty;

    public string Units { get; set; } = string.Empty;

    public string ReferenceRange { get; set; } = string.Empty;

    public string Flag { get; set; } = string.Empty;
}