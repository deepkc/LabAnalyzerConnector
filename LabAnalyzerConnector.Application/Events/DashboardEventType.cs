namespace LabAnalyzerConnector.Application.Events;

public enum DashboardEventType
{
    AnalyzerConnected,

    AnalyzerDisconnected,

    AnalyzerReconnecting,

    MessageReceived,

    MessageSent,

    ResultSaved,

    OrderReceived,

    OrderSent,

    Error,

    Warning,

    Information
}