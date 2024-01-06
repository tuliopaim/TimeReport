using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using TimeReport.Persistence;
using TimeReport.Shared;

namespace TimeReport.Features.TimeEntryFeatures.GetTimeEntryReports;

public record GetTimeEntryReportRequest(DateTimeOffset? From, DateTimeOffset? To);

public class GetTimeEntryReportHandler : Endpoint<GetTimeEntryReportRequest, Results<Ok<ReportsDto>, ProblemDetails>>
{
    private readonly ILogger<GetTimeEntryReportHandler> _logger;
    private readonly ICurrentUser _currentUser;
    private readonly TimeReportContext _timeReportContext;

    public GetTimeEntryReportHandler(
        ILogger<GetTimeEntryReportHandler> logger,
        ICurrentUser currentUser,
        TimeReportContext timeReportContext)
    {
        _logger = logger;
        _currentUser = currentUser;
        _timeReportContext = timeReportContext;
    }

    public override void Configure()
    {
        Post("/api/time-entry");
    }

    public override async Task<Results<Ok<ReportsDto>, ProblemDetails>> ExecuteAsync(GetTimeEntryReportRequest req, CancellationToken ct)
    {
        var companyId = _currentUser.CompanyId!.Value;

        var timeEntries = await _timeReportContext
            .TimeEntries
            .AsNoTrackingWithIdentityResolution()
            .Where(x => x.Employee!.CompanyId == companyId)
            .Select(x => new ReportDto
            {
                Id = x.Id,
                StartDate = x.k,
                EndDate = x.EndDate,
                RequestedBy = x.RequestedByEmployee!.Name,
            })
            .ToListAsync(cancellationToken);

        return new ReportsDto
        {
            Reports = timeEntries
        };
    }
}
