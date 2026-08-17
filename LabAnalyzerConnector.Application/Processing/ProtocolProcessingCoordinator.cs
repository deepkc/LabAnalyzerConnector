using LabAnalyzerConnector.Application.Events;
using LabAnalyzerConnector.Application.Orders;
using LabAnalyzerConnector.Application.Processing.Events;
using LabAnalyzerConnector.Application.Results;
using LabAnalyzerConnector.Communication.Managers;
using LabAnalyzerConnector.Core.Abstractions;
using LabAnalyzerConnector.Domain.Entities;
using LabAnalyzerConnector.Protocols.ASTM;
using LabAnalyzerConnector.Protocols.ASTM.Models;
using LabAnalyzerConnector.Protocols.Models;
using System.Threading;

namespace LabAnalyzerConnector.Application.Processing;

public sealed class ProtocolProcessingCoordinator
{
    private readonly ProtocolMessageProcessingService
        _processingService;

    private readonly AstmOrderQueryHandler
        _orderQueryHandler;

    private readonly AstmOrderResponseBuilder
        _orderResponseBuilder;

    private readonly IAnalyzerConnectionManager
        _connectionManager;

    private readonly LabResultPersistenceService
        _resultPersistenceService;


    public event EventHandler<
        NormalizedMessageProcessedEventArgs>?
        MessageProcessed;

    public event EventHandler<
        ProtocolProcessingErrorEventArgs>?
        ProcessingError;

    private readonly ResultMatchingService
    _resultMatchingService;

    private readonly DashboardEventBus
        _dashboardEventBus;


    public ProtocolProcessingCoordinator(
     ProtocolRouter protocolRouter,
     ProtocolMessageProcessingService processingService,
     AstmOrderQueryHandler orderQueryHandler,
     AstmOrderResponseBuilder orderResponseBuilder,
     IAnalyzerConnectionManager connectionManager,
     LabResultPersistenceService resultPersistenceService,
     ResultMatchingService resultMatchingService,
DashboardEventBus dashboardEventBus)
    {
        ArgumentNullException.ThrowIfNull(
            protocolRouter);

        ArgumentNullException.ThrowIfNull(
            processingService);

        ArgumentNullException.ThrowIfNull(
            orderQueryHandler);

        ArgumentNullException.ThrowIfNull(
            orderResponseBuilder);

        ArgumentNullException.ThrowIfNull(
            connectionManager);

        ArgumentNullException.ThrowIfNull(
            resultPersistenceService);

        _resultMatchingService =
    resultMatchingService;

        _dashboardEventBus = dashboardEventBus;

        _processingService =
            processingService;

        _orderQueryHandler =
            orderQueryHandler;

        _orderResponseBuilder =
            orderResponseBuilder;

        _connectionManager =
            connectionManager;

        _resultPersistenceService =
            resultPersistenceService;


        // =====================================================
        // SUBSCRIBE TO PROTOCOL ROUTER
        // =====================================================

        protocolRouter.MessageReceived +=
            OnMessageReceived;
    }


    // =========================================================
    // MESSAGE RECEIVED
    // =========================================================

    private async void OnMessageReceived(
     object? sender,
     ProtocolMessageReceivedEventArgs eventArgs)
    {
        try
        {
            _dashboardEventBus.Publish(
                new DashboardEvent
                {
                    Type = DashboardEventType.MessageReceived,
                    AnalyzerId = eventArgs.AnalyzerId,
                    Message = "Protocol message received."
                });

            System.Diagnostics.Debug.WriteLine(
                "ProtocolProcessingCoordinator -> Message Received");

            // =====================================================
            // ASTM ORDER QUERY
            // =====================================================

            if (eventArgs.ParsedMessage is AstmMessage astmMessage &&
                astmMessage.OrderQuery is not null)
            {
                AstmOrderQuery orderQuery =
                    astmMessage.OrderQuery;

                LabAnalyzerConnector.Core.Models.LabOrder? order =
                    _orderQueryHandler.FindOrder(orderQuery);

                if (order is null)
                    return;

                string response =
                    _orderResponseBuilder.Build(order);

                await _connectionManager.SendAsync(
                    eventArgs.AnalyzerId,
                    response);

                return;
            }

            // =====================================================
            // NORMAL RESULT PROCESSING
            // =====================================================

            IReadOnlyCollection<NormalizedLabMessage> messages =
      await _processingService.ProcessAsync(
            eventArgs);

            System.Diagnostics.Debug.WriteLine(
                $"Normalized Messages = {messages.Count}");

            foreach (NormalizedLabMessage message in messages)
            {
                System.Diagnostics.Debug.WriteLine(
                    $"Results Count = {message.Results.Count}");

                foreach (LabResult result in message.Results)
                {
                    System.Diagnostics.Debug.WriteLine(
                        $"Saving Result -> {result.TestCode} = {result.ResultValue}");

                    await _resultPersistenceService.SaveAsync(result);

                    _dashboardEventBus.Publish(
                        new DashboardEvent
                        {
                            Type = DashboardEventType.ResultSaved,
                            AnalyzerId = result.AnalyzerId,
                            AnalyzerName = message.AnalyzerName,
                            Message = $"{result.TestCode} = {result.ResultValue}"
                        });
                }

                MessageProcessed?.Invoke(
                    this,
                    new NormalizedMessageProcessedEventArgs(message));
            }
        }
        catch (Exception ex)
        {
            _dashboardEventBus.Publish(
                new DashboardEvent
                {
                    Type = DashboardEventType.Error,
                    AnalyzerId = eventArgs.AnalyzerId,
                    Message = ex.Message
                });

            ProcessingError?.Invoke(
                this,
                new ProtocolProcessingErrorEventArgs(
                    eventArgs.AnalyzerId,
                    ex));
        }
    }
}