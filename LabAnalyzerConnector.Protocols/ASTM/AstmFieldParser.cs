namespace LabAnalyzerConnector.Protocols.ASTM;

public static class AstmFieldParser
{
    // =========================================================
    // GET FIRST COMPONENT
    // =========================================================

    public static string? GetFirstComponent(
    string? field)
    {
        if (string.IsNullOrWhiteSpace(field))
        {
            return null;
        }

        string[] components =
            field.Split('^');

        return components[0].Trim();
    }


    // =========================================================
    // GET SECOND COMPONENT
    // =========================================================

    public static string? GetSecondComponent(
        string? field)
    {
        if (string.IsNullOrWhiteSpace(field))
        {
            return null;
        }

        string[] components =
            field.Split(
                '^',
                StringSplitOptions.None);

        if (components.Length < 2)
        {
            return null;
        }

        string value =
            components[1].Trim();

        return string.IsNullOrWhiteSpace(value)
            ? null
            : value;
    }


    // =========================================================
    // GET THIRD COMPONENT
    // =========================================================

    public static string? GetThirdComponent(
        string? field)
    {
        if (string.IsNullOrWhiteSpace(field))
        {
            return null;
        }

        string[] components =
            field.Split(
                '^',
                StringSplitOptions.None);

        if (components.Length < 3)
        {
            return null;
        }

        string value =
            components[2].Trim();

        return string.IsNullOrWhiteSpace(value)
            ? null
            : value;
    }


    // =========================================================
    // GET FOURTH COMPONENT
    // =========================================================

    public static string? GetFourthComponent(
        string? field)
    {
        if (string.IsNullOrWhiteSpace(field))
        {
            return null;
        }

        string[] components =
            field.Split(
                '^',
                StringSplitOptions.None);

        if (components.Length < 4)
        {
            return null;
        }

        string value =
            components[3].Trim();

        return string.IsNullOrWhiteSpace(value)
            ? null
            : value;
    }


    // =========================================================
    // GET LAST NON-EMPTY COMPONENT
    // =========================================================

    public static string? GetLastNonEmptyComponent(
        string? field)
    {
        if (string.IsNullOrWhiteSpace(field))
        {
            return null;
        }

        string[] components =
            field.Split(
                '^',
                StringSplitOptions.None);

        for (
            int index =
                components.Length - 1;
            index >= 0;
            index--)
        {
            string value =
                components[index].Trim();

            if (!string.IsNullOrWhiteSpace(value))
            {
                return value;
            }
        }

        return null;
    }
}