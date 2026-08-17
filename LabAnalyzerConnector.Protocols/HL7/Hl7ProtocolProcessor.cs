using LabAnalyzerConnector.Protocols.Abstractions;
using LabAnalyzerConnector.Protocols.HL7.Models;
using LabAnalyzerConnector.Protocols.HL7.Parsing;
using LabAnalyzerConnector.Protocols.Models;
using LabAnalyzerConnector.Protocols.HL7.Framing;

namespace LabAnalyzerConnector.Protocols.HL7;

public sealed class Hl7ProtocolProcessor
    : IProtocolProcessor
{
    private readonly Hl7MessageFramer _framer;
    private readonly Hl7Parser _parser;

    public string ProtocolName => "HL7";

    public event EventHandler<ProtocolMessageReceivedEventArgs>?
        MessageReceived;

    public event EventHandler<ProtocolErrorEventArgs>?
        ErrorOccurred;

    public Hl7ProtocolProcessor(
        Hl7MessageFramer framer,
        Hl7Parser parser)
    {
        _framer = framer;
        _parser = parser;
    }

    public void ProcessData(
    Guid analyzerId,
    string data)
    {
        try
        {
            Console.WriteLine("========================================");
            Console.WriteLine("HL7 PROCESSOR");
            Console.WriteLine("========================================");
            Console.WriteLine($"Received Bytes = {data.Length}");

            Console.WriteLine("========== HL7 BYTE DEBUG ==========");

            foreach (char c in data)
            {
                Console.Write($"{(int)c:X2} ");
            }

            Console.WriteLine();
            Console.WriteLine("====================================");


            Console.WriteLine("========== HL7 PROCESSOR INPUT ==========");
            Console.WriteLine($"Length: {data.Length}");

            if (data.Length > 0)
            {
                Console.WriteLine(
                    $"FIRST BYTE: 0x{(int)data[0]:X2}");

                Console.WriteLine(
                    $"LAST BYTE: 0x{(int)data[^1]:X2}");
            }

            Console.WriteLine(
                $"HAS VT: {data.Contains((char)0x0B)}");

            Console.WriteLine(
                $"HAS FS: {data.Contains((char)0x1C)}");

            Console.WriteLine("==========================================");

            IReadOnlyCollection<string> messages =
     _framer.AddData(data).ToList();

            Console.WriteLine(
                $"Complete HL7 Messages = {messages.Count}");
            System.Diagnostics.Debug.WriteLine(
    $"HL7 Complete Messages = {messages.Count}");

            foreach (string message in messages)
            {

                System.Diagnostics.Debug.WriteLine(
                 "HL7 MESSAGE ENTERED FOREACH");
                Console.WriteLine("----------------------------------------");
                Console.WriteLine("Parsing HL7 Message...");
                Console.WriteLine("----------------------------------------");

                Hl7Message hl7Message =
                    _parser.Parse(message);

                Console.WriteLine(
                    $"Patient = {hl7Message.Patient?.PatientId}");

                Console.WriteLine(
                    $"Sample  = {hl7Message.Order?.SampleId}");

                Console.WriteLine(
                    $"OBX Count = {hl7Message.Observations.Count}");

                MessageReceived?.Invoke(
                    this,
                    new ProtocolMessageReceivedEventArgs(
                        analyzerId,
                        message,
                        hl7Message));
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("HL7 ERROR");
            Console.WriteLine(ex);

            ErrorOccurred?.Invoke(
                this,
                new ProtocolErrorEventArgs(
                    analyzerId,
                    ex));
        }
    }
}