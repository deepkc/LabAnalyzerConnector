using LabAnalyzerConnector.Domain.Entities;

namespace LabAnalyzerConnector.Domain.Abstractions;

public interface ILabResultRepository
{
    Task AddAsync(
        LabResult result,
        CancellationToken cancellationToken = default);

    Task<LabResult?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<LabResult>>
        GetAllAsync(
            CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<LabResult>>
        GetBySampleIdAsync(
            string sampleId,
            CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<LabResult>>
        GetByPatientIdAsync(
            string patientId,
            CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<LabResult>>
    GetByAnalyzerIdAsync(
        Guid analyzerId,
        CancellationToken cancellationToken = default);
}