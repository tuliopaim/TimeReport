namespace TimeReport.Features.TimeEntryFeatures.GetTimeEntryReports;

public class ReportsDto
{
    public List<ReportDto> Reports { get; set; } = new();
}

public class ReportDto
{
    public required Guid Id { get; init; }
    public required DateOnly StartDate { get; init; }
    public required DateOnly EndDate { get; init; }
    public required string RequestedBy { get; init; }
}
