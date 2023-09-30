namespace TimeReport.Entities;

public sealed class Employee : IAuditableEntity
{
    public Employee(
        Guid userId,
        Guid companyId,
        string firstName,
        string lastName)
    {
        UserId = userId;
        CompanyId = companyId;
        FirstName = firstName;
        LastName = lastName;
    }

    public Guid Id { get; set; } = Guid.NewGuid();
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public DateTimeOffset CreatedAt { get; } = default;
    public DateTimeOffset? UpdatedAt { get; } = default;

    public Guid UserId { get; private set; }
    public User User { get; private set; }

    public Guid CompanyId { get; }
}