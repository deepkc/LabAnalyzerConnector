using LabAnalyzerSimulator.Database.Entities;

namespace LabAnalyzerSimulator.Repositories;

public interface IResultRepository
{
    Task<IReadOnlyCollection<ResultEntity>>
        GetResultsByBarcodeAsync(
            string barcode);

    Task AddAsync(
        ResultEntity result);

    Task UpdateAsync(
        ResultEntity result);

    Task DeleteAsync(
        Guid id);
}