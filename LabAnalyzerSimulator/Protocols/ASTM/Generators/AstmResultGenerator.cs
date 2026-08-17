using LabAnalyzerSimulator.Models;

namespace LabAnalyzerSimulator.Protocols.ASTM.Generators;

public sealed class AstmResultGenerator
{
    private readonly AstmRealResultLoader _loader =
        new();

    public IReadOnlyCollection<AnalyzerResult> GenerateResults(
        string barcode)
    {
        return _loader.Load(barcode);
    }
}