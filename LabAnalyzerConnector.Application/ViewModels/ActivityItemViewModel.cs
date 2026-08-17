namespace LabAnalyzerConnector.Application.ViewModels;

public sealed class ActivityItemViewModel
{
    public DateTime Time { get; set; }

    public string Analyzer { get; set; } = "";

    public string Message { get; set; } = "";

    public string Type { get; set; } = "";
}