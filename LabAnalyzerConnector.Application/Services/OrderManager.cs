using System.Collections.Concurrent;
using LabAnalyzerConnector.Core.Models;

namespace LabAnalyzerConnector.Application.Services;

public sealed class OrderManager
{
    private readonly ConcurrentDictionary<string, LabOrder> _orders = new(
        StringComparer.OrdinalIgnoreCase);


    // =========================================================
    // ADD ORDER
    // =========================================================

    public void AddOrder(LabOrder order)
    {
        if (order is null)
        {
            throw new ArgumentNullException(nameof(order));
        }

        if (string.IsNullOrWhiteSpace(order.Barcode))
        {
            throw new ArgumentException(
                "Order barcode cannot be empty.",
                nameof(order));
        }

        if (!_orders.TryAdd(
                order.Barcode,
                order))
        {
            throw new InvalidOperationException(
                $"An order with barcode '{order.Barcode}' already exists.");
        }
    }


    // =========================================================
    // FIND ORDER BY BARCODE
    // =========================================================

    public bool TryGetOrderByBarcode(
        string barcode,
        out LabOrder? order)
    {
        if (string.IsNullOrWhiteSpace(barcode))
        {
            order = null;

            return false;
        }

        return _orders.TryGetValue(
            barcode.Trim(),
            out order);
    }


    // =========================================================
    // GET ORDER BY BARCODE
    // =========================================================

    public LabOrder GetOrderByBarcode(
        string barcode)
    {
        if (string.IsNullOrWhiteSpace(barcode))
        {
            throw new ArgumentException(
                "Barcode cannot be empty.",
                nameof(barcode));
        }

        if (!_orders.TryGetValue(
                barcode.Trim(),
                out LabOrder? order))
        {
            throw new InvalidOperationException(
                $"No order found for barcode '{barcode}'.");
        }

        return order;
    }


    // =========================================================
    // REMOVE ORDER
    // =========================================================

    public bool RemoveOrder(
        string barcode)
    {
        if (string.IsNullOrWhiteSpace(barcode))
        {
            return false;
        }

        return _orders.TryRemove(
            barcode.Trim(),
            out _);
    }


    // =========================================================
    // UPDATE STATUS
    // =========================================================

    public bool UpdateStatus(
        string barcode,
        string status)
    {
        if (string.IsNullOrWhiteSpace(barcode))
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(status))
        {
            return false;
        }

        if (!_orders.TryGetValue(
                barcode.Trim(),
                out LabOrder? order))
        {
            return false;
        }

        order.Status = status;

        return true;
    }


    // =========================================================
    // GET ALL ORDERS
    // =========================================================

    public IReadOnlyCollection<LabOrder> GetAllOrders()
    {
        return _orders.Values.ToList();
    }


    // =========================================================
    // GET PENDING ORDERS
    // =========================================================

    public IReadOnlyCollection<LabOrder> GetPendingOrders()
    {
        return _orders.Values
            .Where(order =>
                string.Equals(
                    order.Status,
                    "Pending",
                    StringComparison.OrdinalIgnoreCase))
            .ToList();
    }
}