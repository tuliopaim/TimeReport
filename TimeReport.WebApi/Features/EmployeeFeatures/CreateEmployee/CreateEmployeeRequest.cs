namespace TimeReport.Features.EmployeeFeatures.CreateEmployee;

public record CreateEmployeeRequest(
    string? Username,
    string? FirstName,
    string? LastName,
    string? Password,
    Guid? CompanyId);