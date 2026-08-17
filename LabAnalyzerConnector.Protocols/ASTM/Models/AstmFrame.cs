namespace LabAnalyzerConnector.Protocols.ASTM.Models;

public sealed class AstmFrame
{
    public int FrameNumber { get; }

    public string Data { get; }

    public bool IsLastFrame { get; }

    public AstmFrame(
        int frameNumber,
        string data,
        bool isLastFrame)
    {
        FrameNumber = frameNumber;

        Data = data;

        IsLastFrame = isLastFrame;
    }
}