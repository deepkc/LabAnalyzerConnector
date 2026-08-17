using LabAnalyzerConnector.Domain.Entities;
using LabAnalyzerConnector.Mapping.Services;
using LabAnalyzerConnector.Protocols.Models;
using LabAnalyzerConnector.Application.Normalization;

namespace LabAnalyzerConnector.Application.Processing;

public sealed class ProtocolMessageProcessingService
{
    private readonly IEnumerable<INormalizer> _normalizers;
    private readonly NormalizedMessageProcessingService _mappingService;

    public ProtocolMessageProcessingService(
        IEnumerable<INormalizer> normalizers,
        NormalizedMessageProcessingService mappingService)
    {
        _normalizers = normalizers;
        _mappingService = mappingService;
    }

    public async Task<IReadOnlyCollection<NormalizedLabMessage>> ProcessAsync(
      ProtocolMessageReceivedEventArgs eventArgs,
      CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(eventArgs);

        INormalizer? normalizer =
            _normalizers.FirstOrDefault(
                n => n.CanNormalize(
                    eventArgs.ParsedMessage));

        if (normalizer is null)
        {
            throw new InvalidOperationException(
                $"No normalizer found for parsed message type '{eventArgs.ParsedMessage.GetType().Name}'.");
        }

        var normalizedMessages =
            normalizer.Normalize(
                eventArgs.AnalyzerId,
                eventArgs.ParsedMessage);

        var processedMessages =
            new List<NormalizedLabMessage>();

        foreach (
            NormalizedLabMessage normalizedMessage
            in normalizedMessages)
        {
            normalizedMessage.RawMessage =
                eventArgs.RawMessage;

            normalizedMessage.ReceivedAtUtc =
                eventArgs.ReceivedAtUtc;

            NormalizedLabMessage processedMessage =
      await _mappingService.ProcessAsync(
          normalizedMessage,
          cancellationToken);

            processedMessages.Add(
                processedMessage);
        }

        return processedMessages;
    }
}