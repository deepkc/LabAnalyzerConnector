using LabAnalyzerConnector.Communication.Managers;
using LabAnalyzerConnector.Core.Configuration;
using LabAnalyzerConnector.Core.Services;

namespace LabAnalyzerConnector.Application.Services;

public sealed class AnalyzerConnectionCoordinator
{
    private readonly AnalyzerManager _analyzerManager;

    private readonly ConnectionManager _connectionManager;


    // =========================================================
    // CONSTRUCTOR
    // =========================================================

    public AnalyzerConnectionCoordinator(
        AnalyzerManager analyzerManager,
        ConnectionManager connectionManager)
    {
        _analyzerManager =
            analyzerManager;

        _connectionManager =
            connectionManager;
    }


    // =========================================================
    // CONNECT ONE ANALYZER
    // =========================================================

    public async Task ConnectAsync(
        Guid analyzerId,
        CancellationToken cancellationToken = default)
    {
        AnalyzerConfiguration? configuration =
            _analyzerManager.GetAnalyzer(
                analyzerId);


        // -----------------------------------------------------
        // Analyzer must exist
        // -----------------------------------------------------

        if (configuration is null)
        {
            throw new InvalidOperationException(
                $"Analyzer '{analyzerId}' was not found.");
        }


        // -----------------------------------------------------
        // Disabled analyzers cannot connect
        // -----------------------------------------------------

        if (!configuration.IsEnabled)
        {
            return;
        }


        // -----------------------------------------------------
        // Already registered
        //
        // Do not create a duplicate connection
        // -----------------------------------------------------

        if (_connectionManager.TryGetConnection(
                analyzerId,
                out _))
        {
            return;
        }


        // -----------------------------------------------------
        // Create and connect
        // -----------------------------------------------------

        await _connectionManager.AddAndConnectAsync(
            analyzerId,
            configuration,
            cancellationToken);
    }


    // =========================================================
    // DISCONNECT ONE ANALYZER
    // =========================================================

    public async Task DisconnectAsync(
        Guid analyzerId)
    {
        await _connectionManager.DisconnectAsync(
            analyzerId);
    }


    // =========================================================
    // RECONNECT ONE ANALYZER
    // =========================================================

    public async Task ReconnectAsync(
        Guid analyzerId,
        CancellationToken cancellationToken = default)
    {
        // -----------------------------------------------------
        // Remove existing connection completely
        // -----------------------------------------------------

        await _connectionManager.RemoveAsync(
            analyzerId);


        // -----------------------------------------------------
        // Create a completely new connection
        // -----------------------------------------------------

        await ConnectAsync(
            analyzerId,
            cancellationToken);
    }


    // =========================================================
    // CONNECT ALL AUTO-CONNECT ANALYZERS
    // =========================================================

    public async Task ConnectAllAsync(
        CancellationToken cancellationToken = default)
    {
        IReadOnlyCollection<AnalyzerConfiguration>
            analyzers =
                _analyzerManager.GetAnalyzers();


        foreach (
            AnalyzerConfiguration analyzer
            in analyzers)
        {
            if (!analyzer.IsEnabled ||
                !analyzer.AutoConnect)
            {
                continue;
            }


            try
            {
                await ConnectAsync(
                    analyzer.AnalyzerId,
                    cancellationToken);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception)
            {
                // -------------------------------------------------
                // Important:
                //
                // One analyzer failing must not stop the
                // remaining analyzers from starting.
                // -------------------------------------------------
            }
        }
    }


    // =========================================================
    // DISCONNECT ALL ANALYZERS
    // =========================================================

    public async Task DisconnectAllAsync()
    {
        IReadOnlyCollection<AnalyzerConfiguration>
            analyzers =
                _analyzerManager.GetAnalyzers();


        foreach (
            AnalyzerConfiguration analyzer
            in analyzers)
        {
            try
            {
                await DisconnectAsync(
                    analyzer.AnalyzerId);
            }
            catch
            {
                // Continue disconnecting remaining analyzers.
            }
        }
    }

    public async Task SendAsync(
    Guid analyzerId,
    string data,
    CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(data))
        {
            throw new ArgumentException(
                "Data cannot be empty.",
                nameof(data));
        }

        await _connectionManager.SendAsync(
            analyzerId,
            data,
            cancellationToken);
    }
}