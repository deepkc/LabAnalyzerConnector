using LabAnalyzerSimulator.Database.Entities;
using LabAnalyzerSimulator.Models;

namespace LabAnalyzerSimulator.Mapping;

public static class ResultEntityMapper
{
    public static IReadOnlyCollection<AnalyzerResult> Map(
        IReadOnlyCollection<ResultEntity> entities)
    {
        return entities
            .Select(entity => new AnalyzerResult
            {
                Barcode = entity.Barcode,
                TestCode = entity.TestCode,
                Result = entity.Result,
                Units = entity.Units,
                ReferenceRange = entity.ReferenceRange,
                Flag = entity.Flag
            })
            .ToList();
    }
}