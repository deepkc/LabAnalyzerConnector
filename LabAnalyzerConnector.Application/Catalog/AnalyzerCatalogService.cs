using LabAnalyzerConnector.Application.Catalog.Manufacturers;
using LabAnalyzerConnector.Core.Enums;

namespace LabAnalyzerConnector.Application.Catalog;

public sealed class AnalyzerCatalogService
    : IAnalyzerCatalogService
{
    private readonly List<AnalyzerCatalogItem> _catalog =
    AbbottCatalog.Get().ToList();

    public IReadOnlyCollection<AnalyzerCatalogItem> GetAll()
    {
        return _catalog;
    }

    public IReadOnlyCollection<string> GetManufacturers()
    {
        var manufacturers = _catalog
            .Select(x => x.Manufacturer)
            .Distinct()
            .OrderBy(x => x)
            .ToList();

        manufacturers.Add("Custom");

        return manufacturers;
    }

    public IReadOnlyCollection<string> GetModels(string manufacturer)
    {
        return _catalog
            .Where(x => x.Manufacturer == manufacturer)
            .Select(x => x.Model)
            .Distinct()
            .OrderBy(x => x)
            .ToList();
    }

    public IReadOnlyCollection<AnalyzerCatalogItem> GetByManufacturer(
        string manufacturer)
    {
        return _catalog
            .Where(x => x.Manufacturer == manufacturer)
            .OrderBy(x => x.Model)
            .ToList();
    }

    public AnalyzerCatalogItem? Get(
        string manufacturer,
        string model)
    {
        return _catalog.FirstOrDefault(x =>
            x.Manufacturer == manufacturer &&
            x.Model == model);
    }
}