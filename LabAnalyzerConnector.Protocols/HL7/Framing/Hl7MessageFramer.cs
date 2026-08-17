using System.Text;

namespace LabAnalyzerConnector.Protocols.HL7.Framing;

public sealed class Hl7MessageFramer
{
    private readonly StringBuilder _buffer = new();

    private const char VT = (char)0x0B;
    private const char FS = (char)0x1C;
    private const char CR = (char)0x0D;

    public IEnumerable<string> AddData(string data)
    {
        if (string.IsNullOrEmpty(data))
            yield break;

        _buffer.Append(data);

        while (true)
        {
            string current = _buffer.ToString();

            // =================================================
            // 1. Standard MLLP:
            //
            // VT + HL7 MESSAGE + FS + CR
            // =================================================

            int start = current.IndexOf(VT);

            if (start >= 0)
            {
                int end = current.IndexOf(
                    FS,
                    start + 1);

                if (end < 0)
                    yield break;

                string message = current.Substring(
                    start + 1,
                    end - start - 1);

                // Remove through FS
                int removeLength = end + 1;

                // If CR immediately follows FS, remove it too.
                if (removeLength < current.Length &&
                    current[removeLength] == CR)
                {
                    removeLength++;
                }

                _buffer.Remove(
                    0,
                    removeLength);

                if (!string.IsNullOrWhiteSpace(message))
                    yield return message;

                continue;
            }

            // =================================================
            // 2. Raw HL7:
            //
            // MSH + HL7 MESSAGE + FS
            //
            // Your current analyzer data appears to use this
            // form after the TCP layer.
            // =================================================

            int rawEnd = current.IndexOf(FS);

            if (rawEnd >= 0)
            {
                string message = current
                    .Substring(0, rawEnd)
                    .Trim();

                int removeLength = rawEnd + 1;

                // Optional CR after FS
                if (removeLength < current.Length &&
                    current[removeLength] == CR)
                {
                    removeLength++;
                }

                _buffer.Remove(
                    0,
                    removeLength);

                if (!string.IsNullOrWhiteSpace(message))
                    yield return message;

                continue;
            }

            // =================================================
            // 3. Message is incomplete.
            //
            // Keep it in the buffer because the next TCP
            // receive may contain the rest of the message.
            // =================================================

            yield break;
        }
    }

    public void Clear()
    {
        _buffer.Clear();
    }
}