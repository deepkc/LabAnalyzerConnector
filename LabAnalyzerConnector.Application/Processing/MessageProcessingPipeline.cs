using LabAnalyzerConnector.Domain.Entities;
using LabAnalyzerConnector.Mapping.Abstractions;
using LabAnalyzerConnector.Protocols.Abstractions;
using LabAnalyzerConnector.Protocols.ASTM.Models;
using LabAnalyzerConnector.Protocols.Models;

namespace LabAnalyzerConnector.Application.Processing;

public sealed class MessageProcessingPipeline
    : IMessageProcessingPipeline
{
    private readonly IProtocolProcessor _protocolProcessor;

    private readonly ITestCodeMapper _testCodeMapper;

    public MessageProcessingPipeline(
        IProtocolProcessor protocolProcessor,
        ITestCodeMapper testCodeMapper)
    {
        _protocolProcessor = protocolProcessor;
        _testCodeMapper = testCodeMapper;

        _protocolProcessor.MessageReceived +=
            OnProtocolMessageReceived;
    }

    public Task ProcessIncomingAsync(
        Guid analyzerId,
        string rawMessage,
        CancellationToken cancellationToken = default)
    {
        _protocolProcessor.ProcessData(
            analyzerId,
            rawMessage);

        return Task.CompletedTask;
    }

    private void OnProtocolMessageReceived(
        object? sender,
        ProtocolMessageReceivedEventArgs e)
    {
        if (e.ParsedMessage is AstmMessage astmMessage)
        {
            ProcessAstmMessage(
                e.AnalyzerId,
                astmMessage);
        }
    }

    private void ProcessAstmMessage(
        Guid analyzerId,
        AstmMessage message)
    {
        // Next step
    }
}