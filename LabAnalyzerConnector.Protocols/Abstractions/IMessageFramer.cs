namespace LabAnalyzerConnector.Protocols.Abstractions;

public interface IMessageFramer
{
    IEnumerable<string> AddData(string data);

    void Reset();
}