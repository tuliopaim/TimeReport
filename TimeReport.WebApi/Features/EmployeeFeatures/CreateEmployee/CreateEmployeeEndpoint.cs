using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using TimeReport.Entities;
using TimeReport.Persistence;

namespace TimeReport.Features.EmployeeFeatures.CreateEmployee;

public class CreateEmployeeEndpoint
    : Endpoint<
        CreateEmployeeRequest,
        Results<Ok<Guid>, ProblemDetails>>
{
    private readonly TimeReportContext _timeReportContext;
    private readonly UserManager<User> _userManager;

    public CreateEmployeeEndpoint(
        TimeReportContext timeReportContext,
        UserManager<User> userManager)
    {
        _timeReportContext = timeReportContext;
        _userManager = userManager;
    }

    public override void Configure()
    {
        Post("/api/employee");
        AllowAnonymous();
    }

    public override async Task<Results<Ok<Guid>, ProblemDetails>> ExecuteAsync(
        CreateEmployeeRequest req, CancellationToken ct)
    {
        await _timeReportContext.Database.BeginTransactionAsync(ct);

        var user = new User()
        {
            UserName = req.Username
        };

        var result = await _userManager.CreateAsync(user, req.Password!);

        if (!result.Succeeded)
        {
            return Errors(result, ct);
        }

        var employee = new Employee(
            user.Id,
            req.CompanyId!.Value,
            req.FirstName!,
            req.LastName!);

        _timeReportContext.Add(employee);

        await _timeReportContext.SaveChangesAsync(ct);
        await _timeReportContext.Database.CommitTransactionAsync(ct);

        return TypedResults.Ok(user.Id);
    }

    private ProblemDetails Errors(IdentityResult result, CancellationToken ct)
    {
        foreach (var error in result.Errors)
        {
            AddError(error.Description, error.Code);
        }

        return new ProblemDetails(ValidationFailures, 422);
    }
}