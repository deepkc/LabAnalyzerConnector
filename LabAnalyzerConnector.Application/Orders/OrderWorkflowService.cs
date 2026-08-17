using LabAnalyzerConnector.Core.Models;

namespace LabAnalyzerConnector.Application.Orders;

public sealed class OrderWorkflowService
{
    private readonly BidirectionalOrderService
        _bidirectionalOrderService;


    public OrderWorkflowService(
        BidirectionalOrderService bidirectionalOrderService)
    {
        _bidirectionalOrderService =
            bidirectionalOrderService
            ?? throw new ArgumentNullException(
                nameof(bidirectionalOrderService));
    }


    // =========================================================
    // RECEIVE ORDER FROM LIS / HIS
    // =========================================================

    public void ReceiveOrder(
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

        _bidirectionalOrderService.StoreOrder(
            order);
    }


    // =========================================================
    // ANALYZER REQUESTS ORDER BY BARCODE
    // =========================================================

    public LabOrder? GetOrderForAnalyzer(
        string barcode)
    {
        if (string.IsNullOrWhiteSpace(
                barcode))
        {
            return null;
        }

        return _bidirectionalOrderService
            .FindOrderByBarcode(
                barcode);
    }


    // =========================================================
    // CHECK WHETHER ORDER EXISTS
    // =========================================================

    public bool HasOrder(
        string barcode)
    {
        return _bidirectionalOrderService
            .HasOrder(
                barcode);
    }


    // =========================================================
    // GET TESTS REQUESTED FOR BARCODE
    // =========================================================

    public IReadOnlyCollection<string>
        GetTestsForAnalyzer(
            string barcode)
    {
        return _bidirectionalOrderService
            .GetOrderedTests(
                barcode);
    }

    public void UpdateOrder(LabOrder order)
    {
        if (order is null)
            throw new ArgumentNullException(nameof(order));

        _bidirectionalOrderService.UpdateOrder(order);
    }
}