using LabAnalyzerConnector.Application.Orders;
using LabAnalyzerConnector.Core.Abstractions;
using LabAnalyzerConnector.Core.Models;
using LabAnalyzerConnector.Core.Services;

namespace LabAnalyzerConnector.Tests;

public sealed class AnalyzerOrderQueryServiceTests
{
    [Fact]
    public void SendOrderQuery_ShouldSendOrderToAnalyzer()
    {
        // =====================================================
        // Arrange
        // =====================================================

        Guid analyzerId =
            Guid.NewGuid();

        string barcode =
            "0279070002";

        var orderService =
            new OrderService(
                new FakeOrderRepository());

        var orderSender =
            new FakeAnalyzerOrderSender();

        var service =
            new AnalyzerOrderQueryService(
                orderService,
                orderSender);


        // =====================================================
        // Act
        // =====================================================

        service.SendOrderQuery(
            analyzerId,
            barcode);


        // =====================================================
        // Assert
        // =====================================================

        Assert.Equal(
            analyzerId,
            orderSender.LastAnalyzerId);

        Assert.Equal(
            barcode,
            orderSender.LastBarcode);
    }


    [Fact]
    public void SendOrderQuery_ShouldRejectEmptyAnalyzerId()
    {
        // =====================================================
        // Arrange
        // =====================================================

        var orderService =
            new OrderService(
                new FakeOrderRepository());

        var orderSender =
            new FakeAnalyzerOrderSender();

        var service =
            new AnalyzerOrderQueryService(
                orderService,
                orderSender);


        // =====================================================
        // Act & Assert
        // =====================================================

        Assert.Throws<ArgumentException>(
            () =>
                service.SendOrderQuery(
                    Guid.Empty,
                    "0279070002"));
    }


    [Fact]
    public void SendOrderQuery_ShouldRejectEmptyBarcode()
    {
        // =====================================================
        // Arrange
        // =====================================================

        var orderService =
            new OrderService(
                new FakeOrderRepository());

        var orderSender =
            new FakeAnalyzerOrderSender();

        var service =
            new AnalyzerOrderQueryService(
                orderService,
                orderSender);


        // =====================================================
        // Act & Assert
        // =====================================================

        Assert.Throws<ArgumentException>(
            () =>
                service.SendOrderQuery(
                    Guid.NewGuid(),
                    ""));
    }


    [Fact]
    public void SendOrderQuery_ShouldRejectWhitespaceBarcode()
    {
        // =====================================================
        // Arrange
        // =====================================================

        var orderService =
            new OrderService(
                new FakeOrderRepository());

        var orderSender =
            new FakeAnalyzerOrderSender();

        var service =
            new AnalyzerOrderQueryService(
                orderService,
                orderSender);


        // =====================================================
        // Act & Assert
        // =====================================================

        Assert.Throws<ArgumentException>(
            () =>
                service.SendOrderQuery(
                    Guid.NewGuid(),
                    "   "));
    }


    // =========================================================
    // FAKE ORDER SENDER
    // =========================================================

    private sealed class FakeAnalyzerOrderSender :
        IAnalyzerOrderSender
    {
        public Guid LastAnalyzerId
        {
            get;
            private set;
        }

        public string? LastBarcode
        {
            get;
            private set;
        }


        public void SendOrder(
            Guid analyzerId,
            string barcode)
        {
            LastAnalyzerId =
                analyzerId;

            LastBarcode =
                barcode;
        }
    }


    // =========================================================
    // FAKE ORDER REPOSITORY
    // =========================================================

    private sealed class FakeOrderRepository : IOrderRepository
    {
        public void Add(LabOrder order)
        {
            // Not required for these tests.
        }

        public void Update(LabOrder order)
        {
            // Not required for these tests.
        }

        public IReadOnlyCollection<LabOrder> GetAll()
        {
            return Array.Empty<LabOrder>();
        }

        public LabOrder? GetByBarcode(string barcode)
        {
            return null;
        }

        public bool Remove(Guid id)
        {
            return false;
        }
    }


}