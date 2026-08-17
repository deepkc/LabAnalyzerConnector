using System.Text;
using LabAnalyzerConnector.Protocols.Abstractions;
using LabAnalyzerConnector.Protocols.ASTM.Models;

namespace LabAnalyzerConnector.Protocols.ASTM;

public sealed class AstmMessageFramer : IMessageFramer
{
    private readonly StringBuilder _buffer = new();

    private readonly StringBuilder _messageBuffer = new();

    public IEnumerable<string> AddData(string data)
    {
        if (string.IsNullOrEmpty(data))
        {
            yield break;
        }

        _buffer.Append(data);

        while (true)
        {
            string current = _buffer.ToString();

            // =====================================================
            // WAIT FOR STX
            // =====================================================

            int stxIndex =
                current.IndexOf(
                    AstmControlCharacters.STX);

            if (stxIndex < 0)
            {
                yield break;
            }

            // Remove anything before STX.
            if (stxIndex > 0)
            {
                _buffer.Remove(0, stxIndex);

                current = _buffer.ToString();
            }

            // =====================================================
            // FIND ETX / ETB
            // =====================================================

            int etxIndex =
                current.IndexOf(
                    AstmControlCharacters.ETX,
                    1);

            int etbIndex =
                current.IndexOf(
                    AstmControlCharacters.ETB,
                    1);

            int frameEnd;

            if (etxIndex >= 0 &&
                (etbIndex < 0 ||
                 etxIndex < etbIndex))
            {
                frameEnd = etxIndex;
            }
            else if (etbIndex >= 0)
            {
                frameEnd = etbIndex;
            }
            else
            {
                // We don't have a complete frame yet.
                yield break;
            }

            // =====================================================
            // ASTM FRAME STRUCTURE
            //
            // STX
            // frame number + record
            // ETX
            // checksum 2 chars
            // CR
            // LF
            //
            // Therefore after ETX we need:
            //
            // 2 checksum characters
            // CR
            // LF
            // =====================================================

            int requiredLength =
                frameEnd + 5;

            if (current.Length < requiredLength)
            {
                yield break;
            }

            string frame =
                current.Substring(
                    0,
                    requiredLength);

            _buffer.Remove(
                0,
                requiredLength);

            // =====================================================
            // ADD FRAME TO COMPLETE ASTM MESSAGE
            // =====================================================

            _messageBuffer.Append(frame);

            // =====================================================
            // CHECK FOR EOT
            // =====================================================

            current = _buffer.ToString();

            int eotIndex =
                current.IndexOf(
                    AstmControlCharacters.EOT);

            if (eotIndex >= 0)
            {
                // Add everything up to EOT.
                if (eotIndex > 0)
                {
                    _messageBuffer.Append(
                        current.Substring(
                            0,
                            eotIndex));
                }

                // Remove everything through EOT.
                _buffer.Remove(
                    0,
                    eotIndex + 1);

                // =================================================
                // COMPLETE ASTM TRANSMISSION
                // =================================================

                string completeMessage =
                    _messageBuffer.ToString();

                _messageBuffer.Clear();

                if (!string.IsNullOrWhiteSpace(
                        completeMessage))
                {
                    yield return completeMessage;
                }

                continue;
            }

            // =====================================================
            // NO EOT YET
            //
            // Continue processing another ASTM frame.
            // =====================================================
        }
    }

    public void Reset()
    {
        _buffer.Clear();
        _messageBuffer.Clear();
    }
}