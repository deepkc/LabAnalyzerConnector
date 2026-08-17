using System.Text;
using LabAnalyzerConnector.Protocols.Abstractions;

namespace LabAnalyzerConnector.Protocols.Framing;

public sealed class DelimiterMessageFramer : IMessageFramer
{
    private readonly StringBuilder _buffer = new();

    private readonly string _delimiter;

    public DelimiterMessageFramer(
        string delimiter)
    {
        if (string.IsNullOrEmpty(delimiter))
        {
            throw new ArgumentException(
                "Delimiter cannot be empty.",
                nameof(delimiter));
        }

        _delimiter = delimiter;
    }

    public IEnumerable<string> AddData(
        string data)
    {
        if (string.IsNullOrEmpty(data))
        {
            yield break;
        }

        _buffer.Append(data);

        while (true)
        {
            string currentBuffer =
                _buffer.ToString();

            int delimiterIndex =
                currentBuffer.IndexOf(
                    _delimiter,
                    StringComparison.Ordinal);

            if (delimiterIndex < 0)
            {
                yield break;
            }

            int messageLength =
                delimiterIndex +
                _delimiter.Length;

            string message =
                currentBuffer.Substring(
                    0,
                    messageLength);

            _buffer.Remove(
                0,
                messageLength);

            if (!string.IsNullOrEmpty(message))
            {
                yield return message;
            }
        }
    }

    public void Reset()
    {
        _buffer.Clear();
    }
}