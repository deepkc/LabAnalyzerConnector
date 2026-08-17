using LabAnalyzerSimulator.Database.Entities;

namespace LabAnalyzerSimulator.Repositories;

public interface IOrderRepository
{
    Task<IReadOnlyCollection<OrderEntity>>
        GetOrdersByBarcodeAsync(
            string barcode);

    Task AddAsync(
        OrderEntity order);

    Task UpdateAsync(
        OrderEntity order);

    Task DeleteAsync(
        Guid id);
}