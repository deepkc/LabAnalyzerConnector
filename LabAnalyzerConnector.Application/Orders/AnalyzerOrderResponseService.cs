using LabAnalyzerConnector.Core.Models;

namespace LabAnalyzerConnector.Application.Orders;

public sealed class AnalyzerOrderResponseService
{
    private readonly OrderWorkflowService
        _orderWorkflowService;


    public AnalyzerOrderResponseService(
        OrderWorkflowService orderWorkflowService)
    {
        _orderWorkflowService =
            orderWorkflowService
            ?? throw new ArgumentNullException(
                nameof(orderWorkflowService));
    }


    // =========================================================
    // GET ORDER FOR ANALYZER
    // =========================================================

    public LabOrder? GetOrder(
        string barcode)
    {
        if (string.IsNullOrWhiteSpace(barcode))
        {
            return null;
        }

        return _orderWorkflowService
            .GetOrderForAnalyzer(
                barcode.Trim());
    }


    // =========================================================
    // GET TESTS FOR ANALYZER
    // =========================================================

    public IReadOnlyCollection<string>
        GetTests(
            string barcode)
    {
        if (string.IsNullOrWhiteSpace(barcode))
        {
            return Array.Empty<string>();
        }

        return _orderWorkflowService
            .GetTestsForAnalyzer(
                barcode.Trim());
    }


    // =========================================================
    // CHECK ORDER
    // =========================================================

    public bool HasOrder(
        string barcode)
    {
        if (string.IsNullOrWhiteSpace(barcode))
        {
            return false;
        }

        return _orderWorkflowService
            .HasOrder(
                barcode.Trim());
    }
}