using LabAnalyzerConnector.Core.Abstractions;
using LabAnalyzerConnector.Protocols.ASTM;
using System;

using LabAnalyzerConnector.Infrastructure.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;



namespace LabAnalyzerConnector.Tests;

public sealed class AstmOrderSenderTests
{
    [Fact]
    public void BuildOrderQuery_ShouldCreateQueryForBarcode()
    {
        // =====================================================
        // Arrange
        // =====================================================

        var messageBuilder =
            new AstmOrderMessageBuilder();

        var connectionManager =
            new FakeAnalyzerConnectionManager();

        var sender =
            new AstmOrderSender(
                messageBuilder,
                connectionManager);

        string barcode =
            "0279070002";


        // =====================================================
        // Act
        // =====================================================

        string message =
            sender.BuildOrderQuery(
                barcode);


        // =====================================================
        // Assert
        // =====================================================

        Assert.Contains(
            "H|\\^&",
            message);

        Assert.Contains(
            "Q|1|0279070002",
            message);

        Assert.Contains(
            "L|1",
            message);
    }


    [Fact]
    public void SendOrder_ShouldSendMessageToCorrectAnalyzer()
    {
        // =====================================================
        // Arrange
        // =====================================================

        Guid analyzerId =
            Guid.NewGuid();

        string barcode =
            "0279070002";

        var messageBuilder =
            new AstmOrderMessageBuilder();

        var connectionManager =
            new FakeAnalyzerConnectionManager();

        var sender =
            new AstmOrderSender(
                messageBuilder,
                connectionManager);


        // =====================================================
        // Act
        // =====================================================

        sender.SendOrder(
            analyzerId,
            barcode);


        // =====================================================
        // Assert
        // =====================================================

        Assert.Equal(
            analyzerId,
            connectionManager.LastAnalyzerId);

        Assert.NotNull(
            connectionManager.LastData);

        Assert.Contains(
            "Q|1|0279070002",
            connectionManager.LastData!);
    }


    [Fact]
    public void SendOrder_ShouldRejectEmptyAnalyzerId()
    {
        // =====================================================
        // Arrange
        // =====================================================

        var messageBuilder =
            new AstmOrderMessageBuilder();

        var connectionManager =
            new FakeAnalyzerConnectionManager();

        var sender =
            new AstmOrderSender(
                messageBuilder,
                connectionManager);


        // =====================================================
        // Act & Assert
        // =====================================================

        Assert.Throws<ArgumentException>(
            () =>
                sender.SendOrder(
                    Guid.Empty,
                    "0279070002"));
    }


    [Fact]
    public void SendOrder_ShouldRejectEmptyBarcode()
    {
        // =====================================================
        // Arrange
        // =====================================================

        var messageBuilder =
            new AstmOrderMessageBuilder();

        var connectionManager =
            new FakeAnalyzerConnectionManager();

        var sender =
            new AstmOrderSender(
                messageBuilder,
                connectionManager);


        // =====================================================
        // Act & Assert
        // =====================================================

        Assert.Throws<ArgumentException>(
            () =>
                sender.SendOrder(
                    Guid.NewGuid(),
                    ""));
    }


    // =========================================================
    // FAKE CONNECTION MANAGER
    // =========================================================

    private sealed class FakeAnalyzerConnectionManager :
        IAnalyzerConnectionManager
    {
        public Guid LastAnalyzerId { get; private set; }

        public string? LastData { get; private set; }


        public Task SendAsync(
            Guid analyzerId,
            string data,
            CancellationToken cancellationToken = default)
        {
            LastAnalyzerId =
                analyzerId;

            LastData =
                data;

            return Task.CompletedTask;
        }
    }

    [Fact]
    public async Task DependencyInjection_ShouldResolveAstmOrderSender()
    {
        // =====================================================
        // Arrange
        // =====================================================

        var services =
            new ServiceCollection();

        services.AddLabAnalyzerConnector();

        await using ServiceProvider provider =
            services.BuildServiceProvider();


        // =====================================================
        // Act
        // =====================================================

        IAnalyzerOrderSender sender =
            provider.GetRequiredService<
                IAnalyzerOrderSender>();


        // =====================================================
        // Assert
        // =====================================================

        Assert.NotNull(
            sender);

        Assert.IsType<
            AstmOrderSender>(
            sender);
    }
}