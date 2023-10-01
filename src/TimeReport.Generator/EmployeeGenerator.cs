using Bogus;
using TimeReport.Shared;

namespace TimeReport.Generator;

public class CreateEmployeeRequest
{
    public Guid Id { get; set; }
    public string? Username { get; init; }
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public string? Password { get; init; }
    public Guid? CompanyId { get; init; }
    public int Type { get; init; }
    public List<DateTimeOffset>? Entries { get; init; }
}

public class EmployeeGenerator
{
    private static readonly Random Random = new Random();
    private static readonly Faker Faker = new Faker();

    public static List<CreateEmployeeRequest> CreateEmployees(Guid companyId, int numEmployees)
    {
        var employees = new List<CreateEmployeeRequest>();

        for (var i = 0; i < numEmployees; i++)
        {
            var entries = GenerateRandomEntries();

            var firstName = Faker.Name.FirstName();
            var lastName = Faker.Name.LastName();
            var userName = Faker.Internet.UserName(firstName, lastName);

            employees.Add(new CreateEmployeeRequest
            {
                CompanyId = companyId,
                Username = userName.Length > 16 ? userName.Substring(0, 16) : userName,
                FirstName = firstName,
                LastName = lastName,
                Password = "123456",
                Type = Faker.PickRandom(0, 1),
                Entries = entries
            });
        }

        return employees;
    }

    private static List<DateTimeOffset> GenerateRandomEntries()
    {
        var entries = new List<DateTimeOffset>();

        var firstDayOfMonth = DateTime.Today.ToDateOnly().FirstDayOfMonth();
        var lastDayOfMonth = DateTime.Today.ToDateOnly().LastDayOfMonth();

        for (var day = firstDayOfMonth; day <= lastDayOfMonth; day = day.AddDays(1))
        {
            if (day.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)
            {
                continue;
            }

            // 90% of the time, generate entries
            if (Random.NextDouble() > 0.9)
            {
                continue;
            }

            GenerateRandomDailyEntries(day, entries);
        }

        return entries;
    }

    private static void GenerateRandomDailyEntries(
        DateOnly date,
        List<DateTimeOffset> entries)
    {
        // 85% of the time, generate correct entries
        if (Random.NextDouble() < 0.8)
        {
            GenerateCorrectEntries(date, entries);
            return;
        }

        int numEntries = Random.Next(2, 7);

        var startTime = new TimeOnly(Random.Next(6, 10), 0);
        var startEntry = new DateTimeOffset(date.ToDateTime(startTime));

        entries.Add(startEntry);

        var workedHours = Random.Next(2, 4);
        var workedMinutes = Random.Next(20, 40);
        for (var i = 0; i < numEntries - 1; i++)
        {
            if (i % 2 == 0)
            {
                var newEntry = startEntry
                    .AddHours(workedHours)
                    .AddMinutes(workedMinutes);

                entries.Add(newEntry);

                startEntry = newEntry;

                continue;
            }

            entries.Add(startEntry.AddHours(Random.Next(1, 2)));
        }

        entries.Sort();
    }

    private static void GenerateCorrectEntries(DateOnly date, List<DateTimeOffset> entries)
    {
        var dateTime = date.ToDateTime(new TimeOnly(7, 30).AddMinutes(Random.Next(-30, 30)));

        var workdayStart = new DateTimeOffset(dateTime);

        entries.Add(workdayStart);

        var secondEntry = workdayStart.AddHours(3).AddMinutes(Random.Next(-30, 30));
        entries.Add(secondEntry);

        var thirdEntry = secondEntry.AddHours(1).AddMinutes(Random.Next(0, 35));
        entries.Add(thirdEntry);

        var fourthEntry = thirdEntry.AddHours(5).AddMinutes(Random.Next(-15, 15));
        entries.Add(fourthEntry);
    }
}