using System.Collections.Generic;
using System.Linq;

namespace LabAnalyzerConnector.Application.Profiles;

public sealed class AnalyzerProfileRepository
    : IAnalyzerProfileRepository
{
    private readonly List<AnalyzerProfile> _profiles
        = new();

    public IReadOnlyCollection<AnalyzerProfile> GetAll()
    {
        return _profiles;
    }

    public AnalyzerProfile? Get(
        string manufacturer,
        string model)
    {
        return _profiles.FirstOrDefault(p =>
            p.Manufacturer == manufacturer &&
            p.Model == model);
    }

    public void Add(
        AnalyzerProfile profile)
    {
        _profiles.Add(profile);
    }

    public void Remove(
        string manufacturer,
        string model)
    {
        AnalyzerProfile? profile =
            Get(manufacturer, model);

        if (profile != null)
        {
            _profiles.Remove(profile);
        }
    }
}