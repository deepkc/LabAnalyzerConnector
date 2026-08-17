namespace LabAnalyzerConnector.Core.Abstractions;

public interface IAnalyzerOrderSender
{
    void SendOrder(
        Guid analyzerId,
        string barcode);
}