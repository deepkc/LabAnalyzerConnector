using LabAnalyzerConnector.Core.Abstractions;
using LabAnalyzerConnector.Core.Models;
using LabAnalyzerConnector.Core.Services;

namespace LabAnalyzerConnector.Application.Orders;

public sealed class AnalyzerOrderQueryService
{
    private readonly OrderService _orderService;

    private readonly IAnalyzerOrderSender
        _orderSender;


    public AnalyzerOrderQueryService(
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
            barcode.Trim());
    }


    // =========================================================
    // GET TESTS FOR BARCODE
    // =========================================================

    public IReadOnlyCollection<string>
        GetOrderedTestsByBarcode(
            string barcode)
    {
        LabOrder? order =
            FindOrderByBarcode(
                barcode);

        if (order is null)
        {
            return Array.Empty<string>();
        }

        return order.OrderedTests
            .AsReadOnly();
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
    // SEND ORDER QUERY TO ANALYZER
    // =========================================================

    public void SendOrderQuery(
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

        _orderSender.SendOrder(
            analyzerId,
            barcode.Trim());
    }
}