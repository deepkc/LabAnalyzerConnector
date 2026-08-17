namespace LabAnalyzerConnector.Application.Profiles;

public sealed class AnalyzerResultProfile
{
    public bool SupportsResultCorrection { get; init; }

    public bool SupportsDeltaResults { get; init; }

    public bool SupportsReferenceRanges { get; init; }

    public bool SupportsResultFlags { get; init; }
}