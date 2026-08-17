using LabAnalyzerConnector.Core.Configuration;
using LabAnalyzerConnector.Core.Services;
using LabAnalyzerConnector.Protocols.Abstractions;
using LabAnalyzerConnector.Protocols.Models;
using LabAnalyzerConnector.Core.Enums;

namespace LabAnalyzerConnector.Application.Processing;

public sealed class ProtocolRouter
{
    private readonly AnalyzerConfigurationService
        _configurationService;

    private readonly IEnumerable<IProtocolProcessor>
        _protocolProcessors;

    public event EventHandler<ProtocolMessageReceivedEventArgs>?
        MessageReceived;

    public event EventHandler<ProtocolErrorEventArgs>?
        ErrorOccurred;

    public ProtocolRouter(
        AnalyzerConfigurationService configurationService,
        IEnumerable<IProtocolProcessor> protocolProcessors)
    {
        _configurationService =
            configurationService;

        _protocolProcessors =
            protocolProcessors;

        foreach (IProtocolProcessor processor
                 in _protocolProcessors)
        {
            processor.MessageReceived +=
                OnMessageReceived;

            processor.ErrorOccurred +=
                OnErrorOccurred;
        }
    }

    public void ProcessData(
        Guid analyzerId,
        string data)
    {

        System.Diagnostics.Debug.WriteLine("==================================");
        System.Diagnostics.Debug.WriteLine("ROUTER RECEIVED:");
        System.Diagnostics.Debug.WriteLine(data);
        System.Diagnostics.Debug.WriteLine("==================================");
        System.Diagnostics.Debug.WriteLine(
    "ProtocolRouter -> ProcessData");
        AnalyzerConfiguration? configuration =
            _configurationService.GetById(
                analyzerId);

        if (configuration is null)
        {
            ErrorOccurred?.Invoke(
                this,
                new ProtocolErrorEventArgs(
                    analyzerId,
                    new InvalidOperationException(
                        $"No analyzer configuration found for analyzer '{analyzerId}'.")));

            return;
        }
        System.Diagnostics.Debug.WriteLine(
    $"Configured Protocol = {configuration.Protocol.ProtocolType}");

        foreach (var p in _protocolProcessors)
        {
            System.Diagnostics.Debug.WriteLine(
                $"Registered Processor = {p.ProtocolName}");
        }

        IProtocolProcessor? processor =
            _protocolProcessors.FirstOrDefault(
                p => string.Equals(
                    p.ProtocolName,
                    configuration.Protocol.ProtocolType.ToString(),
                    StringComparison.OrdinalIgnoreCase));

        if (processor is null)
        {
            ErrorOccurred?.Invoke(
                this,
                new ProtocolErrorEventArgs(
                    analyzerId,
                    new InvalidOperationException(
                        $"No protocol processor found for protocol '{configuration.Protocol.ProtocolType}'.")));

            return;
        }

        processor.ProcessData(
            analyzerId,
            data);

        System.Diagnostics.Debug.WriteLine(
    $"ProtocolRouter -> Using processor: {processor.ProtocolName}");
    }

    private void OnMessageReceived(
        object? sender,
        ProtocolMessageReceivedEventArgs e)
    {
        MessageReceived?.Invoke(
            this,
            e);
    }

    private void OnErrorOccurred(
        object? sender,
        ProtocolErrorEventArgs e)
    {
        ErrorOccurred?.Invoke(
            this,
            e);
    }

    public void ProcessData(
    Guid analyzerId,
    ProtocolType protocolType,
    string data)
    {
        IProtocolProcessor? processor =
            _protocolProcessors.FirstOrDefault(
                p => string.Equals(
                    p.ProtocolName,
                    protocolType.ToString(),
                    StringComparison.OrdinalIgnoreCase));

        if (processor is null)
        {
            ErrorOccurred?.Invoke(
                this,
                new ProtocolErrorEventArgs(
                    analyzerId,
                    new InvalidOperationException(
                        $"No processor registered for protocol '{protocolType}'.")));

            return;
        }

        processor.ProcessData(
            analyzerId,
            data);
    }
}