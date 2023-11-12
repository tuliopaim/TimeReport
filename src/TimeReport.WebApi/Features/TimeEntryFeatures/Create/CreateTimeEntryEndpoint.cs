using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using TimeReport.Entities;
using TimeReport.Persistence;
using TimeReport.Shared;

namespace TimeReport.Features.TimeEntryFeatures.Create;

public record CreateTimeEntryRequest(DateTimeOffset? Time);

public class CreateTimeEntryValidator : Validator<CreateTimeEntryRequest>
{
    public CreateTimeEntryValidator()
    {
        When(x => x.Time != null, () =>
        {
            RuleFor(x => x.Time)
                .LessThan(DateTimeOffset.Now);
        });
    }
}

public class CreateTimeEntryEndpoint : Endpoint<CreateTimeEntryRequest, Results<NoContent, ProblemDetails>>
{
    private readonly ILogger<CreateTimeEntryEndpoint> _logger;
    private readonly ICurrentUser _currentUser;
    private readonly TimeReportContext _timeReportContext;
    private readonly IDateTimeProvider _dateTimeProvider;

    public CreateTimeEntryEndpoint(
        ILogger<CreateTimeEntryEndpoint> logger,
        ICurrentUser currentUser,
        TimeReportContext timeReportContext,
        IDateTimeProvider dateTimeProvider)
    {
        _logger = logger;
        _currentUser = currentUser;
        _timeReportContext = timeReportContext;
        _dateTimeProvider = dateTimeProvider;
    }

    public override void Configure()
    {
        Post("/api/time-entry");
    }

    public override async Task<Results<NoContent, ProblemDetails>> ExecuteAsync(CreateTimeEntryRequest req, CancellationToken ct)
    {
        var employeeId = _currentUser.UserId;

        if (employeeId == null)
        {
            throw new InvalidOperationException("User is not logged in");
        }

        var employeeExist = await _timeReportContext.Employees
            .AnyAsync(x => x.Id == employeeId, ct);

        if (!employeeExist)
        {
            AddError("Employee not found", "EmployeeNotFound");
            return new ProblemDetails(ValidationFailures);
        }

        _logger.LogInformation("Creating time entry for employee {EmployeeId}", employeeId);

        var time = req.Time ?? _dateTimeProvider.UtcNow;

        var timeEntry = new TimeEntry(employeeId.Value, time);

        _timeReportContext.Add(timeEntry);

        await _timeReportContext.SaveChangesAsync(ct);

        return TypedResults.NoContent();
    }
}
