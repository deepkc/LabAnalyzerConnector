namespace LabAnalyzerConnector.Application.Models;

public sealed class ReceivedMessageViewModel
{
    public string Data
    {
        get;
    }

    public DateTime ReceivedAt
    {
        get;
    }

    public string ReceivedAtText =>
        ReceivedAt.ToString(
            "yyyy-MM-dd HH:mm:ss.fff");


    public ReceivedMessageViewModel(
        string data,
        DateTime receivedAt)
    {
        Data =
            data;

        ReceivedAt =
            receivedAt;
    }
}