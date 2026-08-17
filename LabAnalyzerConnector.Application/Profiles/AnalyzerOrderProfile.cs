namespace LabAnalyzerConnector.Application.Profiles;

public sealed class AnalyzerOrderProfile
{
    public bool SupportsOrderDownload { get; init; }

    public bool SupportsHostQuery { get; init; }

    public bool SupportsBarcodeQuery { get; init; }
}