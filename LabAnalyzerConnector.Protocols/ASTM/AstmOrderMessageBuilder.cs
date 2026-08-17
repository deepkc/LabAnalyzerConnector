namespace LabAnalyzerConnector.Protocols.ASTM;

using LabAnalyzerConnector.Core.Models;
using System.Text;

public sealed class AstmOrderMessageBuilder
{
    public string BuildQueryMessage(string barcode)
    {
        if (string.IsNullOrWhiteSpace(barcode))
            throw new ArgumentException(
                "Barcode cannot be empty.",
                nameof(barcode));

        return
            "H|\\^&|||||||||||Q|1\r" +
            $"Q|1|{barcode.Trim()}||||||||||O\r" +
            "L|1";
    }

    public string BuildOrderMessage(
        LabOrder order)
    {
        ArgumentNullException.ThrowIfNull(order);

        var builder = new StringBuilder();

        builder.AppendLine("H|\\^&|||||||||||P|1");

        builder.AppendLine(
            $"P|1||{order.PatientId}||{order.PatientName}");

        builder.AppendLine(
            $"O|1|{order.Barcode}||{string.Join("\\", order.OrderedTests)}|R");

        builder.Append("L|1|N");

        return builder.ToString();
    }
}