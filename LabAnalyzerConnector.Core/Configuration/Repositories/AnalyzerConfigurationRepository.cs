using LabAnalyzerConnector.Core.Configuration;
using LabAnalyzerConnector.Core.Configuration.Storage;

namespace LabAnalyzerConnector.Core.Configuration.Repositories;

public sealed class AnalyzerConfigurationRepository
    : IAnalyzerConfigurationRepository
{
    private readonly IAnalyzerConfigurationStorage _storage;

    private readonly Dictionary<Guid, AnalyzerConfiguration>
        _configurations = new();

    public AnalyzerConfigurationRepository(
        IAnalyzerConfigurationStorage storage)
    {
        ArgumentNullException.ThrowIfNull(storage);

        _storage = storage;
    }

    public IReadOnlyCollection<AnalyzerConfiguration> GetAll()
    {
        return _configurations.Values.ToList();
    }

    public AnalyzerConfiguration? GetById(
        Guid analyzerId)
    {
        _configurations.TryGetValue(
            analyzerId,
            out AnalyzerConfiguration? configuration);

        return configuration;
    }

    public async Task LoadAsync()
    {
        IReadOnlyCollection<AnalyzerConfiguration>
            configurations =
                await _storage.LoadAsync();

        _configurations.Clear();

        foreach (
            AnalyzerConfiguration configuration
            in configurations)
        {
            if (configuration.AnalyzerId ==
                Guid.Empty)
            {
                configuration.AnalyzerId =
                    Guid.NewGuid();
            }

            if (_configurations.ContainsKey(
                    configuration.AnalyzerId))
            {
                throw new InvalidOperationException(
                    $"Duplicate analyzer ID '{configuration.AnalyzerId}' was found.");
            }

            _configurations[
                configuration.AnalyzerId] =
                configuration;
        }
    }

    public async Task AddAsync(
        AnalyzerConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(
            configuration);

        if (configuration.AnalyzerId ==
            Guid.Empty)
        {
            configuration.AnalyzerId =
                Guid.NewGuid();
        }

        if (_configurations.ContainsKey(
                configuration.AnalyzerId))
        {
            throw new InvalidOperationException(
                $"Analyzer with ID '{configuration.AnalyzerId}' already exists.");
        }

        _configurations.Add(
            configuration.AnalyzerId,
            configuration);

        await SaveAsync();
    }

    public async Task UpdateAsync(
        AnalyzerConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(
            configuration);

        if (configuration.AnalyzerId ==
            Guid.Empty)
        {
            throw new ArgumentException(
                "Analyzer ID cannot be empty.",
                nameof(configuration));
        }

        if (!_configurations.ContainsKey(
                configuration.AnalyzerId))
        {
            throw new KeyNotFoundException(
                $"Analyzer '{configuration.AnalyzerId}' was not found.");
        }

        _configurations[
            configuration.AnalyzerId] =
            configuration;

        await SaveAsync();
    }

    public async Task<bool> DeleteAsync(
        Guid analyzerId)
    {
        bool removed =
            _configurations.Remove(
                analyzerId);

        if (!removed)
        {
            return false;
        }

        await SaveAsync();

        return true;
    }

    private async Task SaveAsync()
    {
        await _storage.SaveAsync(
            _configurations.Values.ToList());
    }
}