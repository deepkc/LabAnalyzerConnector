using LabAnalyzerConnector.Core.Configuration;

namespace LabAnalyzerConnector.Core.Configuration.Storage;

public interface IAnalyzerConfigurationStorage
{
    Task<IReadOnlyCollection<AnalyzerConfiguration>> LoadAsync();

    Task SaveAsync(
        IReadOnlyCollection<AnalyzerConfiguration> configurations);
}