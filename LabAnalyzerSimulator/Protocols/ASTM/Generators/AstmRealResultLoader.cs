using System.Text.RegularExpressions;
using LabAnalyzerSimulator.Models;
using System.IO;

namespace LabAnalyzerSimulator.Protocols.ASTM.Generators;

public sealed class AstmRealResultLoader
{
    public IReadOnlyCollection<AnalyzerResult> Load(
        string barcode)
    {
        string file =
            Path.Combine(
                AppContext.BaseDirectory,
                "Data",
                "CBC_Normal.astm");

        string text =
            File.ReadAllText(file);

        var results =
            new List<AnalyzerResult>();

        string[] lines =
            text.Split(
                '\n',
                StringSplitOptions.RemoveEmptyEntries);

        foreach (string rawLine in lines)
        {
            string line =
                rawLine.Trim();

            if (!line.Contains("R|"))
            {
                continue;
            }

            string[] fields =
                line.Split('|');

            if (fields.Length < 7)
            {
                continue;
            }

            string testField =
                fields[2];

            string[] parts =
     testField.Split('^');

            string testCode = "";

            if (parts.Length >= 4)
            {
                testCode = parts[3].Trim();
            }
            else if (parts.Length > 0)
            {
                testCode = parts.Last().Trim();
            }

            results.Add(
                new AnalyzerResult
                {
                    Barcode = barcode,
                    TestCode = testCode,
                    Result = fields[3],
                    Units = fields[4],
                    ReferenceRange = fields[5],
                    Flag = fields[6]
                });
        }

        return results;
    }
}