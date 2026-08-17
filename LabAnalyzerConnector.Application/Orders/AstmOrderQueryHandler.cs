using LabAnalyzerConnector.Core.Models;
using LabAnalyzerConnector.Protocols.ASTM.Models;

namespace LabAnalyzerConnector.Application.Orders;

public sealed class AstmOrderQueryHandler
{
    private readonly OrderWorkflowService
        _orderWorkflowService;


    public AstmOrderQueryHandler(
        OrderWorkflowService orderWorkflowService)
    {
        _orderWorkflowService =
            orderWorkflowService
            ?? throw new ArgumentNullException(
                nameof(orderWorkflowService));
    }


    // =========================================================
    // HANDLE ASTM ORDER QUERY
    // =========================================================

    public LabOrder?
        FindOrder(
            AstmOrderQuery query)
    {
        if (query is null)
        {
            throw new ArgumentNullException(
                nameof(query));
        }


        if (string.IsNullOrWhiteSpace(
                query.SampleId))
        {
            return null;
        }


        return _orderWorkflowService
            .GetOrderForAnalyzer(
                query.SampleId.Trim());
    }


    // =========================================================
    // CHECK WHETHER ORDER EXISTS
    // =========================================================

    public bool
        OrderExists(
            AstmOrderQuery query)
    {
        if (query is null)
        {
            throw new ArgumentNullException(
                nameof(query));
        }


        if (string.IsNullOrWhiteSpace(
                query.SampleId))
        {
            return false;
        }


        return _orderWorkflowService
            .HasOrder(
                query.SampleId.Trim());
    }


    // =========================================================
    // GET ORDERED TESTS
    // =========================================================

    public IReadOnlyCollection<string>
        GetOrderedTests(
            AstmOrderQuery query)
    {
        if (query is null)
        {
            throw new ArgumentNullException(
                nameof(query));
        }


        if (string.IsNullOrWhiteSpace(
                query.SampleId))
        {
            return Array.Empty<string>();
        }


        return _orderWorkflowService
            .GetTestsForAnalyzer(
                query.SampleId.Trim());
    }
}