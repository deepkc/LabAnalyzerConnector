using LabAnalyzerConnector.Core.Configuration;
using LabAnalyzerConnector.Core.Configuration.Repositories;

namespace LabAnalyzerConnector.Core.Services;

public sealed class AnalyzerConfigurationService
    : IAnalyzerConfigurationService
{
    private readonly IAnalyzerConfigurationRepository _repository;

    public AnalyzerConfigurationService(
        IAnalyzerConfigurationRepository repository)
    {
        _repository = repository;
    }

    public IReadOnlyCollection<AnalyzerConfiguration> GetAll()
    {
        return _repository.GetAll();
    }

    public AnalyzerConfiguration? GetById(Guid id)
    {
        return _repository.GetById(id);
    }

    public Task LoadAsync()
    {
        return _repository.LoadAsync();
    }

    public async Task AddAsync(
        AnalyzerConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        await _repository.AddAsync(configuration);
    }

    public async Task UpdateAsync(
        AnalyzerConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        await _repository.UpdateAsync(configuration);
    }

    public Task<bool> DeleteAsync(Guid id)
    {
        return _repository.DeleteAsync(id);
    }
}