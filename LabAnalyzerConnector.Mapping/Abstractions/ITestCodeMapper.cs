using LabAnalyzerConnector.Mapping.Models;

namespace LabAnalyzerConnector.Mapping.Abstractions;

public interface ITestCodeMapper
{
    Task<TestCodeMapping?> FindMappingAsync(
        Guid analyzerId,
        string analyzerTestCode,
        CancellationToken cancellationToken = default);

    Task<string?> MapToStandardCodeAsync(
        Guid analyzerId,
        string analyzerTestCode,
        CancellationToken cancellationToken = default);
}