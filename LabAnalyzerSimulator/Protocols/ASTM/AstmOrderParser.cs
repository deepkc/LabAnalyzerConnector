namespace LabAnalyzerSimulator.Protocols.ASTM;

public sealed class AstmOrderParser
{
    public AstmQueryOrder? Parse(string message)
    {
        if (string.IsNullOrWhiteSpace(message))
            return null;

        string[] lines =
            message.Split(
                new[] { '\r', '\n' },
                StringSplitOptions.RemoveEmptyEntries);

        AstmQueryOrder order =
            new();

        foreach (string line in lines)
        {
            // ==================================================
            // ORDER RECORD
            // ==================================================

            if (line.StartsWith("O|"))
            {
                string[] fields = line.Split('|');

                if (fields.Length > 2)
                {
                    order.Barcode = fields[2];
                }

                return order;
            }

            // ==================================================
            // QUERY RECORD (keep support)
            // ==================================================

            if (line.StartsWith("Q|"))
            {
                string[] fields = line.Split('|');

                if (fields.Length > 2)
                {
                    order.Barcode = fields[2];
                }

                return order;
            }
        }

        return null;
    }
}