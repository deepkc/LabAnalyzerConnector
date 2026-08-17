using LabAnalyzerConnector.Communication.Managers;
using LabAnalyzerConnector.Core.Models;
using LabAnalyzerConnector.Protocols.ASTM;


namespace LabAnalyzerConnector.Application.Transmission;

public sealed class OrderTransmissionService
{
    private readonly ConnectionManager _connectionManager;

    private readonly AstmOrderMessageBuilder _builder;

    public OrderTransmissionService(
        ConnectionManager connectionManager,
        AstmOrderMessageBuilder builder)
    {
        _connectionManager = connectionManager;
        _builder = builder;
    }

    public async Task SendOrderAsync(
    Guid analyzerId,
    LabOrder order)
    {
        if (order == null)
            throw new ArgumentNullException(nameof(order));

        string astmMessage =
    _builder.BuildOrderMessage(order);

        await _connectionManager.SendAsync(
            analyzerId,
            astmMessage);
    }
}