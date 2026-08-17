using LabAnalyzerConnector.Domain.Entities;

namespace LabAnalyzerConnector.Application.Normalization;

public interface INormalizer
{
    bool CanNormalize(object parsedMessage);

    IEnumerable<NormalizedLabMessage> Normalize(
        Guid analyzerId,
        object parsedMessage);
}