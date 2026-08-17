using LabAnalyzerConnector.Mapping.Abstractions;
using LabAnalyzerConnector.Mapping.Models;

namespace LabAnalyzerConnector.Mapping.Services;

public sealed class TestCodeMapper : ITestCodeMapper
{
    private readonly ITestCodeMappingRepository _repository;

    public TestCodeMapper(
        ITestCodeMappingRepository repository)
    {
        _repository =
            repository
            ?? throw new ArgumentNullException(
                nameof(repository));
    }

    // =====================================================
    // FIND MAPPING
    // =====================================================

    public async Task<TestCodeMapping?> FindMappingAsync(
        Guid analyzerId,
        string analyzerTestCode,
        CancellationToken cancellationToken = default)
    {
        if (analyzerId == Guid.Empty)
        {
            return null;
        }

        if (string.IsNullOrWhiteSpace(
                analyzerTestCode))
        {
            return null;
        }

        return await _repository.FindAsync(
            analyzerId,
            analyzerTestCode.Trim(),
            cancellationToken);
    }

    // =====================================================
    // MAP TO STANDARD CODE
    // =====================================================

    public async Task<string?> MapToStandardCodeAsync(
        Guid analyzerId,
        string analyzerTestCode,
        CancellationToken cancellationToken = default)
    {
        TestCodeMapping? mapping =
            await FindMappingAsync(
                analyzerId,
                analyzerTestCode,
                cancellationToken);

        return mapping?.StandardTestCode;
    }
}