using System.Collections.Generic;

namespace LabAnalyzerConnector.Application.Profiles;

public interface IAnalyzerProfileRepository
{
    IReadOnlyCollection<AnalyzerProfile> GetAll();

    AnalyzerProfile? Get(
        string manufacturer,
        string model);

    void Add(
        AnalyzerProfile profile);

    void Remove(
        string manufacturer,
        string model);
}