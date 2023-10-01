using TimeReport.Entities;

namespace TimeReport.Features.EmployeeFeatures.Create;

public record CreateEmployeeRequest(
    string? Username,
    string? FirstName,
    string? LastName,
    string? Password,
    Guid? CompanyId,
    EmployeeType? Type,
    short? DailyHours);