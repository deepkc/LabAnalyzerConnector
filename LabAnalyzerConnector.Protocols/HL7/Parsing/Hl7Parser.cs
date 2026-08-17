using LabAnalyzerConnector.Protocols.HL7.Models;

namespace LabAnalyzerConnector.Protocols.HL7.Parsing;

public sealed class Hl7Parser
{
    public Hl7Message Parse(string message)
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            throw new ArgumentException(
                "HL7 message cannot be empty.",
                nameof(message));
        }

        Hl7Message hl7Message =
            new();

        string normalized =
            NormalizeMessage(message);

        string[] segments =
            normalized.Split(
                '\r',
                StringSplitOptions.RemoveEmptyEntries);

        foreach (string rawSegment in segments)
        {
            string segment =
                rawSegment.Trim();

            if (segment.Length < 3)
            {
                continue;
            }

            string[] fields =
                segment.Split('|');

            switch (fields[0])
            {
                case "MSH":

                    hl7Message.Header =
                        ParseMSH(fields);

                    break;

                case "PID":

                    hl7Message.Patient =
                        ParsePID(fields);

                    break;

                case "OBR":

                    hl7Message.Order =
                        ParseOBR(fields);

                    break;

                case "OBX":

                    hl7Message.Observations.Add(
                        ParseOBX(fields));

                    break;
            }
        }

        return hl7Message;
    }

    private static string NormalizeMessage(
        string message)
    {
        return message
            .Replace("\u000B", "")
            .Replace("\u001C", "")
            .Replace("\r\n", "\r")
            .Trim();
    }

    private static Hl7Header ParseMSH(string[] fields)
    {
        Hl7Header header = new();

        if (fields.Length > 2)
            header.SendingApplication = fields[2];

        if (fields.Length > 3)
            header.SendingFacility = fields[3];

        if (fields.Length > 4)
            header.ReceivingApplication = fields[4];

        if (fields.Length > 5)
            header.ReceivingFacility = fields[5];

        if (fields.Length > 8)
        {
            string[] type = fields[8].Split('^');

            if (type.Length > 0)
                header.MessageType = type[0];

            if (type.Length > 1)
                header.TriggerEvent = type[1];
        }

        if (fields.Length > 9)
            header.MessageControlId = fields[9];

        if (fields.Length > 11)
            header.Version = fields[11];

        if (fields.Length > 6)
        {
            if (DateTime.TryParseExact(
                fields[6],
                "yyyyMMddHHmmss",
                null,
                System.Globalization.DateTimeStyles.None,
                out DateTime dt))
            {
                header.MessageDateTime = dt;
            }
        }

        return header;
    }

    private static Hl7Patient ParsePID(
      string[] fields)
    {
        Hl7Patient patient =
            new();

        // ==========================================
        // PID-3 Patient ID
        // ==========================================

        if (fields.Length > 3)
        {
            patient.PatientId =
                fields[3].Trim();
        }

        // ==========================================
        // PID-5 Patient Name
        // ==========================================

        if (fields.Length > 5)
        {
            patient.PatientName =
                fields[5].Replace("^", " ").Trim();
        }

        // ==========================================
        // PID-7 Date of Birth
        // ==========================================

        if (fields.Length > 7)
        {
            if (DateTime.TryParseExact(
                fields[7],
                "yyyyMMdd",
                null,
                System.Globalization.DateTimeStyles.None,
                out DateTime dob))
            {
                patient.DateOfBirth = dob;
            }
        }

        // ==========================================
        // PID-8 Sex
        // ==========================================

        if (fields.Length > 8)
        {
            patient.Sex =
                fields[8].Trim();
        }

        return patient;
    }

    private static Hl7Order ParseOBR(
    string[] fields)
    {
        Hl7Order order =
            new();

        // ==========================================
        // OBR-3
        // Filler Order Number
        // Usually Sample ID / Barcode
        // ==========================================

        if (fields.Length > 3)
        {
            order.SampleId =
                fields[3].Trim();
        }

        // ==========================================
        // OBR-4
        // Universal Service Identifier
        //
        // Example:
        //
        // 00001^Automated Count^99MRC
        // ==========================================

        if (fields.Length > 4)
        {
            string[] components =
                fields[4].Split('^');

            if (components.Length > 0)
            {
                order.TestCode =
                    components[0];
            }

            if (components.Length > 1)
            {
                order.TestName =
                    components[1];
            }
        }

        // ==========================================
        // OBR-7
        // Collection Date/Time
        // ==========================================

        if (fields.Length > 7)
        {
            if (DateTime.TryParseExact(
                fields[7],
                "yyyyMMddHHmmss",
                null,
                System.Globalization.DateTimeStyles.None,
                out DateTime collectionTime))
            {
                order.CollectionDateTime =
                    collectionTime;
            }
        }

        return order;
    }

    private static Hl7Observation ParseOBX(
     string[] fields)
    {
        Hl7Observation observation =
            new();

        // ==========================================
        // OBX-1
        // Sequence Number
        // ==========================================

        if (fields.Length > 1)
        {
            if (int.TryParse(
                fields[1],
                out int sequence))
            {
                observation.SequenceNumber =
                    sequence;
            }
        }

        // ==========================================
        // OBX-3
        // Observation Identifier
        //
        // 6690-2^WBC^LN
        // ==========================================
        if (fields.Length > 3)
        {
            string[] components =
                fields[3].Split('^');

            // OBX-3.1 = Identifier
            // Example: 6690-2
            if (components.Length > 0)
            {
                observation.LoincCode =
                    components[0];

                observation.TestCode =
                    components[0];
            }

            
          

            // OBX-3.3 = Coding system
            // Example: LN
            if (components.Length > 2)
            {
                observation.CodingSystem =
                    components[2];
            }
        }

        // ==========================================
        // OBX-5
        // Result Value
        // ==========================================

        if (fields.Length > 5)
        {
            observation.Value =
                fields[5].Trim();
        }

        // ==========================================
        // OBX-6
        // Units
        // ==========================================

        if (fields.Length > 6)
        {
            observation.Units =
                fields[6].Trim();
        }

        // ==========================================
        // OBX-7
        // Reference Range
        // ==========================================

        if (fields.Length > 7)
        {
            observation.ReferenceRange =
                fields[7].Trim();
        }

        // ==========================================
        // OBX-8
        // Abnormal Flag
        //
        // H~N
        // L~N
        //
        // We only keep H or L.
        // ==========================================

        if (fields.Length > 8)
        {
            string flag =
                fields[8];

            if (!string.IsNullOrWhiteSpace(flag))
            {
                observation.AbnormalFlag =
                    flag.Split('~')[0];
            }
        }

        // ==========================================
        // OBX-11
        // Result Status
        // ==========================================

        if (fields.Length > 11)
        {
            observation.ResultStatus =
                fields[11].Trim();
        }

        return observation;
    }
}