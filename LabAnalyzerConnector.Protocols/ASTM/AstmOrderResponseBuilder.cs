using LabAnalyzerConnector.Core.Models;

namespace LabAnalyzerConnector.Protocols.ASTM;

public sealed class AstmOrderResponseBuilder
{
    public string Build(
        LabOrder order)
    {
        if (order is null)
        {
            throw new ArgumentNullException(
                nameof(order));
        }

        if (string.IsNullOrWhiteSpace(
                order.Barcode))
        {
            throw new ArgumentException(
                "Order barcode cannot be empty.",
                nameof(order));
        }

        var records =
            new List<string>();


        // =====================================================
        // HEADER
        // =====================================================

        records.Add(
            "H|\\^&|||LabAnalyzerConnector|||||LIS||P|1");


        // =====================================================
        // PATIENT
        // =====================================================

        records.Add(
            $"P|1||{order.PatientId}");


        // =====================================================
        // ORDER
        // =====================================================

        string testCodes =
            string.Join(
                "\\",
                order.OrderedTests
                    .Where(
                        test =>
                            !string.IsNullOrWhiteSpace(
                                test))
                    .Select(
                        test =>
                            $"^^^{test}"));

        records.Add(
            $"O|1|{order.Barcode}||{testCodes}||||||||||||Serum||||||||||F");


        // =====================================================
        // TERMINATION
        // =====================================================

        records.Add(
            "L|1|F");


        // =====================================================
        // BUILD ASTM MESSAGE
        // =====================================================

        return string.Join(
            "\r",
            records)
            + "\r";
    }
}