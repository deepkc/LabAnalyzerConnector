using System.Text.Json;
using System.Text.Json.Serialization;
using LabAnalyzerConnector.Core.Configuration;

namespace LabAnalyzerConnector.Core.Configuration.Storage;

public sealed class JsonAnalyzerConfigurationStorage
    : IAnalyzerConfigurationStorage
{
    private readonly string _filePath;

    private readonly JsonSerializerOptions _jsonOptions =
        new()
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true,
            Converters =
            {
                new JsonStringEnumConverter()
            }
        };

    public JsonAnalyzerConfigurationStorage(
        string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            throw new ArgumentException(
                "File path is required.",
                nameof(filePath));
        }

        _filePath = filePath;
    }

    public async Task<IReadOnlyCollection<AnalyzerConfiguration>>
        LoadAsync()
    {
        if (!File.Exists(_filePath))
        {
            return Array.Empty<AnalyzerConfiguration>();
        }

        await using FileStream stream =
            File.OpenRead(_filePath);

        var configurations =
            await JsonSerializer.DeserializeAsync<
                List<AnalyzerConfiguration>>(
                    stream,
                    _jsonOptions);

        return configurations
            ?? new List<AnalyzerConfiguration>();
    }

    public async Task SaveAsync(
        IReadOnlyCollection<AnalyzerConfiguration>
            configurations)
    {
        string? directory =
            Path.GetDirectoryName(_filePath);

        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }

        await using FileStream stream =
            File.Create(_filePath);

        await JsonSerializer.SerializeAsync(
            stream,
            configurations,
            _jsonOptions);
    }
}