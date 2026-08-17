namespace LabAnalyzerConnector.Application.Profiles;

public sealed class AnalyzerCommunicationProfile
{
    public int DefaultBaudRate { get; init; }

    public int DefaultDataBits { get; init; }

    public string DefaultParity { get; init; } = "None";

    public string DefaultStopBits { get; init; } = "One";

    public int DefaultPort { get; init; }

    public bool AutoReconnect { get; init; }

    public int ReconnectIntervalSeconds { get; init; }

    public int ReadTimeoutMilliseconds { get; init; }

    public int WriteTimeoutMilliseconds { get; init; }
}