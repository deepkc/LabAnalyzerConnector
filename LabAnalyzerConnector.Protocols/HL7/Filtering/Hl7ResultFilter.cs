using LabAnalyzerConnector.Protocols.HL7.Models;

namespace LabAnalyzerConnector.Protocols.HL7.Filtering;

public sealed class Hl7ResultFilter
{
    private static readonly HashSet<string> IgnoredTests =
    [
        "Take Mode",
        "Blood Mode",
        "Test Mode",
        "Ref Group",
        "Project Type",
        "Shelf No",
        "Tube No",
        "Analyzer",
        "Platelet Clump?",
        "PLT Abnormal histogram"
    ];

    private static readonly string[] IgnoredKeywords =
    [
        "Histogram",
        "Scattergram",
        "Meta",
        "dimension",
        "Binary"
    ];

    public List<Hl7Observation> Filter(
        IEnumerable<Hl7Observation> observations)
    {
        List<Hl7Observation> results = new();

        foreach (Hl7Observation observation in observations)
        {
            if (ShouldKeep(observation))
            {
                results.Add(observation);
            }
        }

        return results;
    }

    private static bool ShouldKeep(
        Hl7Observation observation)
    {
        string testCode =
            observation.TestCode ?? "";

        if (IgnoredTests.Contains(testCode))
            return false;

        foreach (string keyword in IgnoredKeywords)
        {
            if (testCode.Contains(
                    keyword,
                    StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
        }

        return true;
    }

    public bool IsResultObservation(Hl7Observation observation)
    {
        return ShouldKeep(observation);
    }
}