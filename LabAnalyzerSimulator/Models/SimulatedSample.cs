namespace LabAnalyzerSimulator.Models;

public sealed class SimulatedSample
{
    public string Barcode { get; set; } = string.Empty;

    public string PatientId { get; set; } = string.Empty;

    public List<SimulatedResult> Results { get; set; } = new();
}