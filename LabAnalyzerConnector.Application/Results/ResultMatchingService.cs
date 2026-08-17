using LabAnalyzerConnector.Application.Orders;
using LabAnalyzerConnector.Core.Models;
using LabAnalyzerConnector.Domain.Entities;

namespace LabAnalyzerConnector.Application.Results;

public sealed class ResultMatchingService
{
    private readonly OrderWorkflowService _orderWorkflowService;

    public ResultMatchingService(
        OrderWorkflowService orderWorkflowService)
    {
        _orderWorkflowService = orderWorkflowService;
    }

    public LabOrder? MatchResult(
        LabResult result)
    {
        if (result is null)
            throw new ArgumentNullException(nameof(result));

        if (string.IsNullOrWhiteSpace(result.SampleId))
            return null;

        LabOrder? order =
            _orderWorkflowService.GetOrderForAnalyzer(
                result.SampleId);

        if (order is null)
            return null;

        order.Status = "Completed";

        order.Status = "Completed";

        _orderWorkflowService.UpdateOrder(order);

        return order;
    }
}