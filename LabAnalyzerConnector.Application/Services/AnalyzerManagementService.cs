using LabAnalyzerConnector.Application.Models;
using LabAnalyzerConnector.Core.Configuration;
using LabAnalyzerConnector.Core.Enums;
using LabAnalyzerConnector.Core.Services;

namespace LabAnalyzerConnector.Application.Services;

public sealed class AnalyzerManagementService
{
    private readonly IAnalyzerManager _analyzerManager;

    public AnalyzerManagementService(
        IAnalyzerManager analyzerManager)
    {
        _analyzerManager = analyzerManager;
    }

    // =========================================================
    // LOAD LIST
    // =========================================================

    public IReadOnlyCollection<AnalyzerListItem> GetAnalyzers()
    {
        return _analyzerManager
            .GetAnalyzers()
            .Select(a => new AnalyzerListItem
            {
                AnalyzerId = a.AnalyzerId,
                Name = a.Name,
                Manufacturer = a.Manufacturer,
                Model = a.Model,
                Protocol = a.Protocol.ProtocolType,
                ConnectionType = a.ConnectionType,
                Enabled = a.IsEnabled,
                Status = ConnectionStatus.Disconnected
            })
            .ToList();
    }

    // =========================================================
    // GET CONFIGURATION
    // =========================================================

    public AnalyzerConfiguration? GetAnalyzer(Guid analyzerId)
    {
        return _analyzerManager
            .GetAnalyzers()
            .FirstOrDefault(x => x.AnalyzerId == analyzerId);
    }

    // =========================================================
    // DELETE
    // =========================================================

    public async Task DeleteAnalyzerAsync(Guid analyzerId)
    {
        await _analyzerManager.RemoveAnalyzerAsync(analyzerId);
    }

    // =========================================================
    // UPDATE
    // =========================================================

    public async Task UpdateAnalyzerAsync(
        AnalyzerConfiguration configuration)
    {
        await _analyzerManager.UpdateAnalyzerAsync(configuration);
    }

    // =========================================================
    // ADD
    // =========================================================

    public async Task AddAnalyzerAsync(
        AnalyzerConfiguration configuration)
    {
        await _analyzerManager.AddAnalyzerAsync(configuration);
    }
}