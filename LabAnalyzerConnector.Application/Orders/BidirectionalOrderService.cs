using LabAnalyzerConnector.Core.Models;
using LabAnalyzerConnector.Core.Services;
using LabAnalyzerConnector.Core.Abstractions;


namespace LabAnalyzerConnector.Application.Orders;

public sealed class BidirectionalOrderService
{
    private readonly OrderService _orderService;

    private readonly IAnalyzerOrderSender
        _orderSender;


    public BidirectionalOrderService(
        OrderService orderService,
        IAnalyzerOrderSender orderSender)
    {
        _orderService =
            orderService
            ?? throw new ArgumentNullException(
                nameof(orderService));

        _orderSender =
            orderSender
            ?? throw new ArgumentNullException(
                nameof(orderSender));
    }


    // =========================================================
    // STORE ORDER
    // =========================================================

    public void StoreOrder(
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

        _orderService.AddOrder(
            order);
    }


    // =========================================================
    // FIND ORDER BY BARCODE
    // =========================================================

    public LabOrder? FindOrderByBarcode(
        string barcode)
    {
        if (string.IsNullOrWhiteSpace(
                barcode))
        {
            return null;
        }

        return _orderService.FindByBarcode(
            barcode);
    }


    // =========================================================
    // CHECK WHETHER ORDER EXISTS
    // =========================================================

    public bool HasOrder(
        string barcode)
    {
        return FindOrderByBarcode(
            barcode) is not null;
    }


    // =========================================================
    // GET TESTS FOR BARCODE
    // =========================================================

    public IReadOnlyCollection<string>
        GetOrderedTests(
            string barcode)
    {
        LabOrder? order =
            FindOrderByBarcode(
                barcode);

        if (order is null)
        {
            return Array.Empty<string>();
        }

        return order.OrderedTests;
    }


    // =========================================================
    // SEND ORDER TO ANALYZER
    // =========================================================

    public void SendOrderToAnalyzer(
        Guid analyzerId,
        string barcode)
    {
        if (analyzerId == Guid.Empty)
        {
            throw new ArgumentException(
                "Analyzer ID cannot be empty.",
                nameof(analyzerId));
        }

        if (string.IsNullOrWhiteSpace(
                barcode))
        {
            throw new ArgumentException(
                "Barcode cannot be empty.",
                nameof(barcode));
        }

        LabOrder? order =
            FindOrderByBarcode(
                barcode);

        if (order is null)
        {
            throw new InvalidOperationException(
                $"No order found for barcode '{barcode}'.");
        }

        _orderSender.SendOrder(
            analyzerId,
            barcode);
    }

    public void UpdateOrder(LabOrder order)
    {
        _orderService.UpdateOrder(order);
    }
}