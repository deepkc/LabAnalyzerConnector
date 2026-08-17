using LabAnalyzerConnector.Domain.Entities;

namespace LabAnalyzerConnector.Application.ViewModels;

public sealed class ResultListItemViewModel
{
    public Guid Id { get; }

    public Guid AnalyzerId { get; }

    public string AnalyzerName { get; }

    public string PatientId { get; }

    public string SampleId { get; }

    public string TestCode { get; }

    public string StandardTestCode { get; }

    public string TestName { get; }

    public string ResultValue { get; }

    public string Units { get; }

    public string ReferenceRange { get; }

    public string AbnormalFlag { get; }

    public DateTime ResultDateTime { get; }

    public DateTime ReceivedAtUtc { get; }

    public string ReceivedAtText =>
        ReceivedAtUtc
            .ToLocalTime()
            .ToString("yyyy-MM-dd HH:mm:ss");

    public ResultListItemViewModel(
        LabResult result)
    {
        Id = result.Id;
        AnalyzerId = result.AnalyzerId;

        AnalyzerName =
            result.AnalyzerName ?? string.Empty;

        PatientId =
            result.PatientId ?? string.Empty;

        SampleId =
            result.SampleId ?? string.Empty;

        TestCode =
            result.TestCode ?? string.Empty;

        StandardTestCode =
            result.StandardTestCode ?? string.Empty;

        TestName =
            result.TestName ?? string.Empty;

        ResultValue =
            result.ResultValue ?? string.Empty;

        Units =
            result.Units ?? string.Empty;

        ReferenceRange =
            result.ReferenceRange ?? string.Empty;

        AbnormalFlag =
            result.AbnormalFlag ?? string.Empty;

        ResultDateTime =
            result.ResultDateTime;

        ReceivedAtUtc =
            result.ReceivedAtUtc;
    }
}