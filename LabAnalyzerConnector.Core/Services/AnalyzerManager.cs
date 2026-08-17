using LabAnalyzerConnector.Core.Configuration;



namespace LabAnalyzerConnector.Core.Services;

public sealed class AnalyzerManager : IAnalyzerManager
{
    private readonly IAnalyzerConfigurationService _configurationService;
    public AnalyzerManager(
     IAnalyzerConfigurationService configurationService)
    {
        _configurationService = configurationService;
    }

    public event EventHandler? AnalyzerCollectionChanged;

    public event EventHandler<AnalyzerConfiguration>? AnalyzerAdded;

    public event EventHandler<AnalyzerConfiguration>? AnalyzerRemoved;

    public event EventHandler<AnalyzerConfiguration>? AnalyzerUpdated;

    public IReadOnlyCollection<AnalyzerConfiguration> GetAnalyzers()
    {
        return _configurationService.GetAll();
    }

    public AnalyzerConfiguration? GetAnalyzer(Guid analyzerId)
    {
        return _configurationService.GetById(analyzerId);
    }

    public async Task LoadAsync()
    {
        await _configurationService.LoadAsync();

        AnalyzerCollectionChanged?.Invoke(
            this,
            EventArgs.Empty);
    }

    public Task SaveAsync()
    {
        // Repository automatically saves after Add, Update and Delete.
        return Task.CompletedTask;
    }

    public async Task AddAnalyzerAsync(
      AnalyzerConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        await _configurationService.AddAsync(configuration);

        AnalyzerAdded?.Invoke(
            this,
            configuration);

        AnalyzerCollectionChanged?.Invoke(
            this,
            EventArgs.Empty);
    }

    public async Task UpdateAnalyzerAsync(
     AnalyzerConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        await _configurationService.UpdateAsync(configuration);

        AnalyzerUpdated?.Invoke(
            this,
            configuration);

        AnalyzerCollectionChanged?.Invoke(
            this,
            EventArgs.Empty);
    }

    public async Task RemoveAnalyzerAsync(Guid analyzerId)
    {
        bool removed =
            await _configurationService.DeleteAsync(analyzerId);

        if (!removed)
        {
            throw new InvalidOperationException(
                "Analyzer not found.");
        }

        AnalyzerConfiguration? configuration =
            _configurationService.GetById(analyzerId);

        if (configuration != null)
        {
            AnalyzerRemoved?.Invoke(
                this,
                configuration);
        }

        AnalyzerCollectionChanged?.Invoke(
            this,
            EventArgs.Empty);
    }

    public Task ConnectAsync(Guid analyzerId)
    {
        throw new NotImplementedException();
    }

    public Task DisconnectAsync(Guid analyzerId)
    {
        throw new NotImplementedException();
    }

    public Task ConnectAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task DisconnectAllAsync()
    {
        throw new NotImplementedException();
    }
}