using LabAnalyzerConnector.Domain.Abstractions;
using LabAnalyzerConnector.Domain.Entities;

namespace LabAnalyzerConnector.Application.Results;

public sealed class LabResultPersistenceService
{
    private readonly ILabResultRepository _repository;

    public LabResultPersistenceService(
        ILabResultRepository repository)
    {
        _repository =
            repository
            ?? throw new ArgumentNullException(
                nameof(repository));
    }

    public Task SaveAsync(
        LabResult result,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(
            result);

        return _repository.AddAsync(
            result,
            cancellationToken);
    }

    public Task<LabResult?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return _repository.GetByIdAsync(
            id,
            cancellationToken);
    }

    public Task<IReadOnlyCollection<LabResult>>
        GetAllAsync(
            CancellationToken cancellationToken = default)
    {
        return _repository.GetAllAsync(
            cancellationToken);
    }

    public Task<IReadOnlyCollection<LabResult>>
        GetBySampleIdAsync(
            string sampleId,
            CancellationToken cancellationToken = default)
    {
        return _repository.GetBySampleIdAsync(
            sampleId,
            cancellationToken);
    }

    public Task<IReadOnlyCollection<LabResult>>
        GetByPatientIdAsync(
            string patientId,
            CancellationToken cancellationToken = default)
    {
        return _repository.GetByPatientIdAsync(
            patientId,
            cancellationToken);
    }


    public Task<IReadOnlyCollection<LabResult>>
    GetByAnalyzerIdAsync(
        Guid analyzerId,
        CancellationToken cancellationToken = default)
    {
        return _repository.GetByAnalyzerIdAsync(
            analyzerId,
            cancellationToken);
    }


 
}