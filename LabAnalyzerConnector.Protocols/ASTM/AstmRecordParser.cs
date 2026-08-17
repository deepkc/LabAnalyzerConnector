using LabAnalyzerConnector.Protocols.ASTM.Models;

namespace LabAnalyzerConnector.Protocols.ASTM;

public sealed class AstmRecordParser
{
    public AstmMessage Parse(
        string message)
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            throw new ArgumentException(
                "ASTM message cannot be empty.",
                nameof(message));
        }

        var result =
            new AstmMessage();

        string normalizedMessage =
            NormalizeMessage(message);

        string[] records =
            normalizedMessage.Split(
                '\r',
                StringSplitOptions.RemoveEmptyEntries);

        foreach (string rawRecord in records)
        {
            string record =
                rawRecord.Trim();

            if (string.IsNullOrWhiteSpace(record))
            {
                continue;
            }

            string[] fields =
                record.Split('|');

            if (fields.Length == 0)
            {
                continue;
            }

            string recordType =
                GetRecordType(
                    fields[0]);

            switch (recordType)
            {
                case "H":

                    result.Header =
                        ParseHeader(
                            record,
                            fields);

                    break;

                case "P":

                    result.Patient =
                        ParsePatient(
                            record,
                            fields);

                    break;

                case "O":

                    result.Order =
                        ParseOrder(
                            record,
                            fields);

                    break;

                case "Q":

                    result.OrderQuery =
                        ParseOrderQuery(
                            record,
                            fields);

                    break;

                case "R":

                    result.Results.Add(
                        ParseResult(
                            record,
                            fields));

                    break;

                case "L":

                    break;
            }
        }

        return result;
    }


    // =========================================================
    // RECORD TYPE
    // =========================================================

    private static string GetRecordType(
        string field)
    {
        if (string.IsNullOrWhiteSpace(field))
        {
            return string.Empty;
        }

        string value =
            field.Trim();

        // Normal ASTM record:
        //
        // O
        // R
        // P
        //
        // ASTM framed record:
        //
        // 3O
        // 4R
        // 2P

        if (value.Length >= 2 &&
            char.IsDigit(value[0]))
        {
            value =
                value.Substring(1);
        }

        return value;
    }


    // =========================================================
    // HEADER
    // =========================================================

    private static AstmHeaderRecord ParseHeader(
        string rawRecord,
        string[] fields)
    {
        var header =
            new AstmHeaderRecord(
                rawRecord);

        if (fields.Length > 4)
        {
            header.SenderName =
                fields[4].Trim();
        }

        if (fields.Length > 11)
        {
            header.SenderVersion =
                fields[11].Trim();
        }

        return header;
    }


    // =========================================================
    // PATIENT
    // =========================================================

    private static AstmPatientRecord ParsePatient(
     string rawRecord,
     string[] fields)
    {
        var patient =
            new AstmPatientRecord(
                rawRecord);

        // P|1|||0279070002|&H&&N&

        // P-4: Patient ID
        if (fields.Length > 4)
        {
            patient.PatientId =
                fields[4].Trim();
        }

        // P-5: Patient Name
        if (fields.Length > 5)
        {
            patient.PatientName =
                fields[5].Trim();
        }

        return patient;
    }

    // =========================================================
    // ORDER
    // =========================================================

    private static AstmOrderRecord ParseOrder(
     string rawRecord,
     string[] fields)
    {
        var order =
            new AstmOrderRecord(
                rawRecord);

        // =========================================================
        // O-3: Sample ID / Barcode
        //
        // Example:
        //
        // O|1|0279070002^M^1|25134
        //
        // or:
        //
        // O|1|   0279070002^M^1|25134
        //
        // Expected SampleId:
        //
        // 0279070002
        // =========================================================

        if (fields.Length > 2)
        {
            string sampleField =
                fields[2];

            if (!string.IsNullOrWhiteSpace(sampleField))
            {
                string[] components =
                    sampleField.Split(
                        '^',
                        StringSplitOptions.None);

                if (components.Length > 0)
                {
                    string sampleId =
                        components[0].Trim();

                    if (!string.IsNullOrWhiteSpace(sampleId))
                    {
                        order.SampleId =
                            sampleId;
                    }
                }
            }
        }


        // =========================================================
        // O-5: Test Code
        //
        // Example:
        //
        // O|1|0279070002^M^1|25134||||||||||||BLOOD^019
        //
        // =========================================================

        if (fields.Length > 4)
        {
            order.TestCode =
                AstmFieldParser.GetLastNonEmptyComponent(
                    fields[4]);
        }


        return order;
    }


    // =========================================================
    // ORDER QUERY
    // =========================================================

    private static AstmOrderQuery ParseOrderQuery(
        string rawRecord,
        string[] fields)
    {
        var query =
            new AstmOrderQuery(
                rawRecord);

        if (fields.Length > 2)
        {
            query.SampleId =
                fields[2].Trim();
        }

        return query;
    }


    // =========================================================
    // RESULT
    // =========================================================

    private static AstmResultRecord ParseResult(
        string rawRecord,
        string[] fields)
    {
        var result =
            new AstmResultRecord(
                rawRecord);

        // =====================================================
        // R-3 TEST IDENTIFIER
        // =====================================================

        if (fields.Length > 2)
        {
            string testField =
                fields[2];

            result.TestCode =
                AstmFieldParser.GetFourthComponent(
                    testField);

            if (string.IsNullOrWhiteSpace(
                    result.TestCode))
            {
                result.TestCode =
                    AstmFieldParser.GetLastNonEmptyComponent(
                        testField);
            }
        }

        // =====================================================
        // R-4 VALUE
        // =====================================================

        if (fields.Length > 3)
        {
            result.Value =
                fields[3].Trim();
        }

        // =====================================================
        // R-5 UNITS
        // =====================================================

        if (fields.Length > 4)
        {
            result.Units =
                fields[4].Trim();
        }

        // =====================================================
        // R-6 REFERENCE RANGE
        // =====================================================

        if (fields.Length > 5)
        {
            result.ReferenceRange =
                fields[5].Trim();
        }

        // =====================================================
        // R-7 ABNORMAL FLAG
        // =====================================================

        if (fields.Length > 6)
        {
            result.AbnormalFlag =
                fields[6].Trim();
        }

        return result;
    }


    // =========================================================
    // NORMALIZE MESSAGE
    // =========================================================

    private static string NormalizeMessage(
        string message)
    {
        return message
            .Replace(
                "\u0002",
                string.Empty)

            .Replace(
                "\u0003",
                string.Empty)

            .Replace(
                "\u0017",
                string.Empty)

            .Replace(
                "\u0004",
                string.Empty)

            .Replace(
                "\u0005",
                string.Empty)

            .Replace(
                "\r\n",
                "\r")

            .Trim();
    }
}