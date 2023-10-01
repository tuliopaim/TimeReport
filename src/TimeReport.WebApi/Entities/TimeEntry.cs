namespace TimeReport.Entities;

public class TimeEntry : IAuditableEntity
{
    public TimeEntry()
    {
    }

    public TimeEntry(Guid employeeId, DateTimeOffset time)
    {
        EmployeeId = employeeId;
        Time = time;
    }

    public Guid Id { get; private set; } = new();

    public DateTimeOffset Time { get; set; }

    public DateTimeOffset CreatedAt { get; } = default;
    public DateTimeOffset? UpdatedAt { get; } = default;

    public Guid EmployeeId { get; private set; }

    public Employee? Employee { get; set; }
}