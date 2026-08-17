using LabAnalyzerConnector.Communication.Managers;
using LabAnalyzerConnector.Core.Abstractions;

namespace LabAnalyzerConnector.Protocols.ASTM;

public sealed class AstmOrderSender : IAnalyzerOrderSender
{
    private readonly AstmOrderMessageBuilder
        _messageBuilder;

    private readonly IAnalyzerConnectionManager
     _connectionManager;

    public AstmOrderSender(
     AstmOrderMessageBuilder messageBuilder,
     IAnalyzerConnectionManager connectionManager)
    {
        _messageBuilder =
            messageBuilder
            ?? throw new ArgumentNullException(
                nameof(messageBuilder));

        _connectionManager =
            connectionManager
            ?? throw new ArgumentNullException(
                nameof(connectionManager));
    }


    public string BuildOrderQuery(
        string barcode)
    {
        return _messageBuilder.BuildQueryMessage(
            barcode);
    }


    public void SendOrder(
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


        // =====================================================
        // Build ASTM Query
        // =====================================================

        string message =
            _messageBuilder.BuildQueryMessage(
                barcode);


        // =====================================================
        // Send through existing connection manager
        // =====================================================

        _connectionManager
            .SendAsync(
                analyzerId,
                message)
            .GetAwaiter()
            .GetResult();
    }
}