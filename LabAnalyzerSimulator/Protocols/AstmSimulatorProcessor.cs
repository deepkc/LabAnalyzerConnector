namespace LabAnalyzerSimulator.Protocols;

public sealed class AstmSimulatorProcessor
{
    public bool IsOrderQuery(
        string message)
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            return false;
        }

        return message.Contains("Q|");
    }

    public string? ExtractBarcode(
        string message)
    {
        if (!IsOrderQuery(message))
        {
            return null;
        }

        string[] records =
            message.Split('\r');

        foreach (string record in records)
        {
            if (!record.StartsWith("Q|"))
            {
                continue;
            }

            string[] fields =
                record.Split('|');

            if (fields.Length > 2)
            {
                return fields[2].Trim();
            }
        }

        return null;
    }
}