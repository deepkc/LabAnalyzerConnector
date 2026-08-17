using System.Collections.Concurrent;
using System.Collections.Generic;
using LabAnalyzerConnector.Core.Models;

namespace LabAnalyzerConnector.Core.Services;

public sealed class InMemoryOrderRepository
    : IOrderRepository
{
    private readonly ConcurrentDictionary<
        Guid,
        LabOrder> _orders = new();


    public void Add(
        LabOrder order)
    {
        if (order is null)
        {
            throw new ArgumentNullException(
                nameof(order));
        }

        if (string.IsNullOrWhiteSpace(
                order.Barcode))
        {
            throw new ArgumentException(
                "Order barcode cannot be empty.",
                nameof(order));
        }

        _orders[order.Id] = order;
    }


    public LabOrder? GetByBarcode(
        string barcode)
    {
        if (string.IsNullOrWhiteSpace(
                barcode))
        {
            return null;
        }

        return _orders.Values
            .FirstOrDefault(order =>
                string.Equals(
                    order.Barcode.Trim(),
                    barcode.Trim(),
                    StringComparison.OrdinalIgnoreCase));
    }


    public IReadOnlyCollection<LabOrder> GetAll()
    {
        return _orders.Values.ToList();
    }


    public bool Remove(
        Guid orderId)
    {
        return _orders.TryRemove(
            orderId,
            out _);
    }

    public void Update(LabOrder order)
    {
        ArgumentNullException.ThrowIfNull(order);

        _orders[order.Id] = order;
    }
}