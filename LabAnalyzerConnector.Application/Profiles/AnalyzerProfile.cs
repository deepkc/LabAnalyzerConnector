using System;

namespace LabAnalyzerConnector.Application.Profiles;

public sealed class AnalyzerProfile
{
    public Guid Id { get; init; }

    public string Manufacturer { get; init; } = string.Empty;

    public string Model { get; init; } = string.Empty;

    public AnalyzerCommunicationProfile Communication { get; init; }
        = new();

    public AnalyzerProtocolProfile Protocol { get; init; }
        = new();

    public AnalyzerParsingProfile Parsing { get; init; }
        = new();

    public AnalyzerOrderProfile Orders { get; init; }
        = new();

    public AnalyzerResultProfile Results { get; init; }
        = new();
}