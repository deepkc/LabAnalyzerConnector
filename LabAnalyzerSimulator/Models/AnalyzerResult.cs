namespace LabAnalyzerSimulator.Models;

public sealed class AnalyzerResult
{
    public string Barcode { get; set; } = "";

    public string TestCode { get; set; } = "";

    public string Result { get; set; } = "";

    public string Units { get; set; } = "";

    public string ReferenceRange { get; set; } = "";

    public string Flag { get; set; } = "N";
}