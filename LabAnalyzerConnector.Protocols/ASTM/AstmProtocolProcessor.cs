using LabAnalyzerConnector.Protocols.Abstractions;
using LabAnalyzerConnector.Protocols.Models;

namespace LabAnalyzerConnector.Protocols.ASTM;

public sealed class AstmProtocolProcessor
    : IProtocolProcessor
{
    private readonly AstmMessageFramer _framer;

    private readonly AstmRecordParser _parser;

    public string ProtocolName => "ASTM";

    public event EventHandler<ProtocolMessageReceivedEventArgs>?
        MessageReceived;

    public event EventHandler<ProtocolErrorEventArgs>?
        ErrorOccurred;

    public AstmProtocolProcessor(
        AstmMessageFramer framer,
        AstmRecordParser parser)
    {
        _framer = framer;
        _parser = parser;
    }

    public void ProcessData(
        Guid analyzerId,
        string data)
    {
        try
        {
            foreach (string frame in _framer.AddData(data))
            {
                // Verify that the frame can be parsed.
                // We'll use the parsed AstmMessage in the next stage.
                var astmMessage =
     _parser.Parse(frame);

                System.Diagnostics.Debug.WriteLine(
    $"Parsed Results = {astmMessage.Results.Count}");


                MessageReceived?.Invoke(
                    this,
                    new ProtocolMessageReceivedEventArgs(
                        analyzerId,
                        frame,
                        astmMessage));
            }
        }
        catch (Exception ex)
        {
            ErrorOccurred?.Invoke(
                this,
                new ProtocolErrorEventArgs(
                    analyzerId,
                    ex));
        }
    }
}