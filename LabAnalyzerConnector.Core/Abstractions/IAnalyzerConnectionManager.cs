namespace LabAnalyzerConnector.Core.Abstractions;

public interface IAnalyzerConnectionManager
{
    Task SendAsync(
        Guid analyzerId,
        string data,
        CancellationToken cancellationToken = default);
}