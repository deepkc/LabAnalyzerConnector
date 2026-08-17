namespace LabAnalyzerConnector.Application.Profiles;

public sealed class AnalyzerParsingProfile
{
    public string SampleIdField { get; init; } = string.Empty;

    public string PatientIdField { get; init; } = string.Empty;

    public string TestCodeField { get; init; } = string.Empty;

    public string ResultField { get; init; } = string.Empty;

    public string UnitsField { get; init; } = string.Empty;

    public string FlagsField { get; init; } = string.Empty;
}