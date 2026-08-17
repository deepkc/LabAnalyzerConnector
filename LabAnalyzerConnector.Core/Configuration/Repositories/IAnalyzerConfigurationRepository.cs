using LabAnalyzerConnector.Core.Configuration;

namespace LabAnalyzerConnector.Core.Configuration.Repositories;

public interface IAnalyzerConfigurationRepository
{
    IReadOnlyCollection<AnalyzerConfiguration> GetAll();

    AnalyzerConfiguration? GetById(Guid id);

    Task LoadAsync();

    Task AddAsync(
        AnalyzerConfiguration configuration);

    Task UpdateAsync(
        AnalyzerConfiguration configuration);

    Task<bool> DeleteAsync(Guid id);
}