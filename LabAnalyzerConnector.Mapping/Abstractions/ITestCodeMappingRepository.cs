using LabAnalyzerConnector.Mapping.Models;

namespace LabAnalyzerConnector.Mapping.Abstractions;

public interface ITestCodeMappingRepository
{
    Task AddAsync(
        TestCodeMapping mapping,
        CancellationToken cancellationToken = default);

    Task UpdateAsync(
        TestCodeMapping mapping,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<TestCodeMapping?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<TestCodeMapping>>
        GetByAnalyzerIdAsync(
            Guid analyzerId,
            CancellationToken cancellationToken = default);

    Task<TestCodeMapping?> FindAsync(
        Guid analyzerId,
        string analyzerTestCode,
        CancellationToken cancellationToken = default);
}