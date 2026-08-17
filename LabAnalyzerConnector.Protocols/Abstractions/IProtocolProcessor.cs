namespace LabAnalyzerConnector.Protocols.Abstractions;
using LabAnalyzerConnector.Protocols.Models;

public interface IProtocolProcessor
{
    string ProtocolName { get; }

    void ProcessData(
        Guid analyzerId,
        string data);

    event EventHandler<ProtocolMessageReceivedEventArgs>?
        MessageReceived;

    event EventHandler<ProtocolErrorEventArgs>?
        ErrorOccurred;
}