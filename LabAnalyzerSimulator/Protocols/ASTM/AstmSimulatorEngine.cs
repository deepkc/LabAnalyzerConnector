using LabAnalyzerSimulator.Protocols.ASTM.Builders;
using LabAnalyzerSimulator.Protocols.ASTM.Generators;

namespace LabAnalyzerSimulator.Protocols.ASTM;

public sealed class AstmSimulatorEngine
{
    private readonly AstmSimulatorProcessor _processor;

    private readonly AstmResultMessageBuilder _builder;

    private readonly AstmResultGenerator _generator;

    public AstmSimulatorEngine(
        AstmSimulatorProcessor processor,
        AstmResultMessageBuilder builder)
    {
        _processor = processor;
        _builder = builder;
        _generator = new AstmResultGenerator();
    }

    public Task<string?> ProcessMessageAsync(
        string message)
    {
        if (!_processor.IsOrderQuery(message))
            return Task.FromResult<string?>(null);

        string? barcode =
            _processor.ExtractBarcode(message);

        if (string.IsNullOrWhiteSpace(barcode))
            return Task.FromResult<string?>(null);

        var results =
            _generator.GenerateResults(barcode);

        string response =
            _builder.BuildResultMessage(results);

        return Task.FromResult<string?>(response);
    }
}