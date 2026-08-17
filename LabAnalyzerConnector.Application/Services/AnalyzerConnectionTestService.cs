using LabAnalyzerConnector.Communication.Factories;
using LabAnalyzerConnector.Core.Configuration;

namespace LabAnalyzerConnector.Application.Services;

public sealed class AnalyzerConnectionTestService
{
    private readonly ConnectionFactory _connectionFactory;

    public AnalyzerConnectionTestService(
        ConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<bool> TestAsync(
        AnalyzerConfiguration configuration,
        CancellationToken cancellationToken = default)
    {
        var connection =
            _connectionFactory.Create(
                Guid.NewGuid(),
                configuration);

        try
        {
            await connection.ConnectAsync(cancellationToken);

            await connection.DisconnectAsync();

            return true;
        }
        catch
        {
            return false;
        }
        finally
        {
            await connection.DisposeAsync();
        }
    }
}