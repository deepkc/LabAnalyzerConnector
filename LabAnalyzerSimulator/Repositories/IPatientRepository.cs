using LabAnalyzerSimulator.Database.Entities;

namespace LabAnalyzerSimulator.Repositories;

public interface IPatientRepository
{
    Task<PatientEntity?> GetByBarcodeAsync(
        string barcode);

    Task<IReadOnlyCollection<PatientEntity>>
        GetAllAsync();

    Task AddAsync(
        PatientEntity patient);

    Task UpdateAsync(
        PatientEntity patient);

    Task DeleteAsync(
        Guid id);
}