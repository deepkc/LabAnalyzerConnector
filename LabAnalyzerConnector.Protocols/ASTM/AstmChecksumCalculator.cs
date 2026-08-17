using System.Text;
using LabAnalyzerConnector.Protocols.ASTM.Models;

namespace LabAnalyzerConnector.Protocols.ASTM;

public static class AstmChecksumCalculator
{
    public static string Calculate(
        string frameData,
        bool isLastFrame)
    {
        if (frameData is null)
        {
            throw new ArgumentNullException(
                nameof(frameData));
        }

        char terminatingCharacter =
            isLastFrame
                ? AstmControlCharacters.ETX
                : AstmControlCharacters.ETB;

        string checksumContent =
            $"{AstmControlCharacters.STX}" +
            frameData +
            terminatingCharacter +
            AstmControlCharacters.CR +
            AstmControlCharacters.LF;

        byte[] bytes =
            Encoding.ASCII.GetBytes(
                checksumContent);

        int checksum = 0;

        foreach (byte value in bytes)
        {
            checksum += value;
        }

        checksum %= 256;

        return checksum.ToString(
            "X2");
    }

    public static bool Validate(
        string frame,
        string receivedChecksum)
    {
        if (string.IsNullOrWhiteSpace(frame))
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(
                receivedChecksum))
        {
            return false;
        }

        bool isLastFrame =
            frame.Contains(
                AstmControlCharacters.ETX);

        string frameData =
            ExtractFrameData(
                frame);

        string calculatedChecksum =
            Calculate(
                frameData,
                isLastFrame);

        return string.Equals(
            calculatedChecksum,
            receivedChecksum,
            StringComparison.OrdinalIgnoreCase);
    }

    private static string ExtractFrameData(
        string frame)
    {
        int stxIndex =
            frame.IndexOf(
                AstmControlCharacters.STX);

        if (stxIndex < 0)
        {
            return frame;
        }

        int start =
            stxIndex + 1;

        int etxIndex =
            frame.IndexOf(
                AstmControlCharacters.ETX,
                start);

        int etbIndex =
            frame.IndexOf(
                AstmControlCharacters.ETB,
                start);

        int end;

        if (etxIndex >= 0 &&
            (etbIndex < 0 ||
             etxIndex < etbIndex))
        {
            end = etxIndex;
        }
        else
        {
            end = etbIndex;
        }

        if (end < 0)
        {
            return frame[start..];
        }

        return frame[
            start..end];
    }
}