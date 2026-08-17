using LabAnalyzerConnector.Core.Models;

namespace LabAnalyzerConnector.Core.Services;

public interface IOrderRepository
{
    void Add(LabOrder order);

    LabOrder? GetByBarcode(
        string barcode);

    IReadOnlyCollection<LabOrder> GetAll();

    bool Remove(
        Guid orderId);

    void Update(LabOrder order);
}