using LabAnalyzerConnector.Core.Models;

namespace LabAnalyzerConnector.Core.Services;

public sealed class OrderService
{
    private readonly IOrderRepository _orderRepository;


    public OrderService(
        IOrderRepository orderRepository)
    {
        _orderRepository =
            orderRepository
            ?? throw new ArgumentNullException(
                nameof(orderRepository));
    }


    public void AddOrder(
        LabOrder order)
    {
        if (order is null)
        {
            throw new ArgumentNullException(
                nameof(order));
        }

        _orderRepository.Add(
            order);
    }


    public LabOrder? FindByBarcode(
        string barcode)
    {
        return _orderRepository.GetByBarcode(
            barcode);
    }


    public IReadOnlyCollection<LabOrder>
        GetAllOrders()
    {
        return _orderRepository.GetAll();
    }


    public bool RemoveOrder(
        Guid orderId)
    {
        return _orderRepository.Remove(
            orderId);
    }

    public void UpdateOrder(LabOrder order)
    {
        if (order is null)
            throw new ArgumentNullException(nameof(order));

        _orderRepository.Update(order);
    }
}