namespace LabAnalyzerConnector.Simulator.Communication.Tcp;

public interface IAnalyzerConnection
{
    event EventHandler<string>? DataReceived;

    bool IsConnected { get; }

    Task StartAsync();

    Task StopAsync();

    Task SendAsync(string message);


}