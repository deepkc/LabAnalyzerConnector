namespace LabAnalyzerConnector.Application.Profiles;

public sealed class AnalyzerProtocolProfile
{
    public bool RequiresAck { get; init; }

    public bool UsesChecksum { get; init; }

    public bool UsesEnqEot { get; init; }

    public bool SupportsOrderQuery { get; init; }

    public bool SupportsResults { get; init; }

    public int RetryCount { get; init; }

    public int AckTimeoutMilliseconds { get; init; }
}