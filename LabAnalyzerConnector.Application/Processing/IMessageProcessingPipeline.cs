using LabAnalyzerConnector.Domain.Entities;

namespace LabAnalyzerConnector.Application.Processing;

public interface IMessageProcessingPipeline
{
    Task ProcessIncomingAsync(
        Guid analyzerId,
        string rawMessage,
        CancellationToken cancellationToken = default);
}