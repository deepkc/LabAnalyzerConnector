using LabAnalyzerConnector.Core.Configuration;

namespace LabAnalyzerConnector.Core.Services;

public interface IAnalyzerManager
{
    // =========================================================
    // Events
    // =========================================================

    event EventHandler? AnalyzerCollectionChanged;

    event EventHandler<AnalyzerConfiguration>? AnalyzerAdded;

    event EventHandler<AnalyzerConfiguration>? AnalyzerRemoved;

    event EventHandler<AnalyzerConfiguration>? AnalyzerUpdated;

    // =========================================================
    // Query
    // =========================================================

    IReadOnlyCollection<AnalyzerConfiguration> GetAnalyzers();

    AnalyzerConfiguration? GetAnalyzer(Guid analyzerId);

    // =========================================================
    // Management
    // =========================================================

    Task AddAnalyzerAsync(
        AnalyzerConfiguration configuration);

    Task UpdateAnalyzerAsync(
        AnalyzerConfiguration configuration);

    Task RemoveAnalyzerAsync(
        Guid analyzerId);

    // =========================================================
    // Persistence
    // =========================================================

    Task LoadAsync();

    Task SaveAsync();

    // =========================================================
    // Connections
    // =========================================================

    Task ConnectAsync(Guid analyzerId);

    Task DisconnectAsync(Guid analyzerId);

    Task ConnectAllAsync();

    Task DisconnectAllAsync();
}