namespace LabAnalyzerConnector.Core.Enums;

public enum AnalyzerStatus
{
    Disabled = 1,
    Stopped = 2,
    Starting = 3,
    Connecting = 4,
    Connected = 5,
    Communicating = 6,
    Disconnected = 7,
    Reconnecting = 8,
    Error = 9
}