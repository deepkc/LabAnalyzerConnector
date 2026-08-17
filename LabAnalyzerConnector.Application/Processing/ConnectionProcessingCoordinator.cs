using LabAnalyzerConnector.Communication.Managers;
using LabAnalyzerConnector.Communication.Managers.Events;

namespace LabAnalyzerConnector.Application.Processing;

public sealed class ConnectionProcessingCoordinator
{
    private readonly ProtocolRouter _protocolRouter;

    public ConnectionProcessingCoordinator(
        ConnectionManager connectionManager,
        ProtocolRouter protocolRouter)
    {
        ArgumentNullException.ThrowIfNull(
            connectionManager);

        ArgumentNullException.ThrowIfNull(
            protocolRouter);

        _protocolRouter =
            protocolRouter;

        connectionManager.DataReceived +=
            OnDataReceived;
    }

    //private void OnDataReceived(
    //    object? sender,
    //    AnalyzerDataReceivedEventArgs eventArgs)
    //{
    //    System.Diagnostics.Debug.WriteLine(
    //"ConnectionProcessingCoordinator -> Data Received");
    //    _protocolRouter.ProcessData(
    //        eventArgs.AnalyzerId,
    //        eventArgs.Data);
    //}

    private void OnDataReceived(
     object? sender,
     AnalyzerDataReceivedEventArgs eventArgs)
    {
        Console.WriteLine();
        Console.WriteLine("========================================");
        Console.WriteLine("CONNECTION COORDINATOR RAW DATA");
        Console.WriteLine("========================================");

        Console.WriteLine(
            $"AnalyzerId : {eventArgs.AnalyzerId}");

        Console.WriteLine(
            $"Length     : {eventArgs.Data?.Length ?? 0}");

        Console.WriteLine(
            $"First Char : {(eventArgs.Data?.Length > 0 ? ((int)eventArgs.Data[0]).ToString("X2") : "NONE")}");

        Console.WriteLine(
            $"Last Char  : {(eventArgs.Data?.Length > 0 ? ((int)eventArgs.Data[^1]).ToString("X2") : "NONE")}");

        Console.WriteLine();
        Console.WriteLine("CHARACTER CODES:");

        if (!string.IsNullOrEmpty(eventArgs.Data))
        {
            foreach (char c in eventArgs.Data)
            {
                Console.Write($"{(int)c:X2} ");
            }
        }

        Console.WriteLine();
        Console.WriteLine();
        Console.WriteLine("TEXT:");

        Console.WriteLine(
            eventArgs.Data);

        Console.WriteLine();
        Console.WriteLine("========================================");

        _protocolRouter.ProcessData(
            eventArgs.AnalyzerId,
            eventArgs.Data);
    }


}