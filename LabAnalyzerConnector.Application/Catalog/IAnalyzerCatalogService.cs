namespace LabAnalyzerConnector.Application.Catalog;

public interface IAnalyzerCatalogService
{
    IReadOnlyCollection<AnalyzerCatalogItem> GetAll();

    IReadOnlyCollection<string> GetManufacturers();

    IReadOnlyCollection<AnalyzerCatalogItem> GetByManufacturer(
        string manufacturer);

    IReadOnlyCollection<string> GetModels(
        string manufacturer);

    AnalyzerCatalogItem? Get(
        string manufacturer,
        string model);
}