namespace TimeReport.Entities;

public sealed class Company : IAuditableEntity
{
    public Company(string name)
    {
        Name = name;
    }

    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; private set; }
    public DateTimeOffset CreatedAt { get; } = default;
    public DateTimeOffset? UpdatedAt { get; } = default;

    private readonly List<Employee> _employees = new();
    public IReadOnlyList<Employee> Employees => _employees;

    public void AddEmployee(Employee employee)
    {
        _employees.Add(employee);
    }
}