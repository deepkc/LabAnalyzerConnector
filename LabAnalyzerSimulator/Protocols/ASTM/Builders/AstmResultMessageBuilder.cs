using System.Text;
using LabAnalyzerSimulator.Models;

namespace LabAnalyzerSimulator.Protocols.ASTM.Builders;

public sealed class AstmResultMessageBuilder
{
    private const char STX = (char)0x02;
    private const char ETX = (char)0x03;
    private const char CR = (char)0x0D;
    private const char LF = (char)0x0A;

    public string BuildResultMessage(
        IReadOnlyCollection<AnalyzerResult> results)
    {
        if (results.Count == 0)
            return string.Empty;

        AnalyzerResult first =
            results.First();

        var sb = new StringBuilder();

        sb.Append(STX);

        sb.Append("1H|\\^&|||LabAnalyzerSimulator|||||P|1");
        sb.Append(CR);

        sb.Append($"2P|1||{first.Barcode}");
        sb.Append(CR);

        sb.Append($"3O|1|{first.Barcode}");
        sb.Append(CR);

        int recordNo = 4;

        foreach (AnalyzerResult result in results)
        {
            sb.Append(
                $"{recordNo}R|1|^^^{result.TestCode}|{result.Result}|{result.Units}|{result.ReferenceRange}|{result.Flag}");

            sb.Append(CR);

            recordNo++;
        }

        sb.Append($"{recordNo}L|1|N");

        sb.Append(CR);

        sb.Append(ETX);

        sb.Append("00");

        sb.Append(CR);

        sb.Append(LF);

        return sb.ToString();
    }
}