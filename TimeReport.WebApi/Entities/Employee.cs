namespace TimeReport.Entities;

public sealed class Employee : IAuditableEntity
{
    public Employee(
        Guid userId,
        Guid companyId,
        string firstName,
        string? lastName = null,
        EmployeeType type = EmployeeType.Regular)
    {
        Id = userId;
        UserId = userId;
        CompanyId = companyId;
        FirstName = firstName;
        LastName = lastName;
        Type = type;
    }

    public Guid Id { get; }
    public string FirstName { get; private set; }
    public string? LastName { get; private set; }
    public EmployeeType Type { get; private set; }
    public DateTimeOffset CreatedAt { get; } = default;
    public DateTimeOffset? UpdatedAt { get; } = default;

    public Guid UserId { get; private set; }
    public User User { get; private set; }

    public Guid CompanyId { get; }
}