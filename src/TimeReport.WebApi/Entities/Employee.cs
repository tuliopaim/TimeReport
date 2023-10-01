namespace TimeReport.Entities;

public sealed class Employee : IAuditableEntity
{
    public Employee()
    {
    }

    public Employee(
        Guid userId,
        Guid companyId,
        string firstName,
        string? lastName = null,
        short? dailyHours = 8,
        EmployeeType type = EmployeeType.Regular)
    {
        Id = userId;
        UserId = userId;
        CompanyId = companyId;
        FirstName = firstName;
        LastName = lastName;
        DailyHours = dailyHours ?? 8;
        Type = type;
    }

    public Guid Id { get; }
    public string FirstName { get; private set; }
    public string? LastName { get; private set; }
    public EmployeeType Type { get; private set; }
    public short DailyHours { get; private set; }
    public DateTimeOffset CreatedAt { get; } = default;
    public DateTimeOffset? UpdatedAt { get; } = default;

    public Guid UserId { get; private set; }
    public User? User { get; private set; }

    public Guid CompanyId { get; }
    public Company Company { get; private set; }

    private readonly List<TimeEntry> _timeEntries = new();
    public IReadOnlyList<TimeEntry> TimeEntries => _timeEntries;

    public void AddTimeEntry(DateTimeOffset time)
    {
        var timeEntry = new TimeEntry(Id, time);

        _timeEntries.Add(timeEntry);
    }
}
