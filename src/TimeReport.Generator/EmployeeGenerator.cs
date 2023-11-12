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
    private static readonly Random Random = new();
    private static readonly Faker Faker = new();

    private readonly static double ProbToHaveEntries = 0.95;
    private readonly static double ProbToHaveCompleteEntries = 0.9;
    private readonly static double ProbToHaveIntervalEntry = 0.7;
    private readonly static double ProbToHaveFirstEntry = 0.75;
    private readonly static int CompleteEntries = 4;

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
                Username = userName.Length > 16 ? userName[..16] : userName,
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

            // 5% of the days will have no entries
            if (!Check(ProbToHaveEntries))
            {
                continue;
            }

            // 10% of the days will incomplete entries
            var dailyEntries = Check(ProbToHaveCompleteEntries)
                ? CreateDailyEntries(day)
                : CreateDailyEntries(day, Random.Next(1, 3));

            entries.AddRange(dailyEntries);
        }

        return entries;
    }

    private static bool Check(double probability) => Random.NextDouble() < probability;

    private static IEnumerable<DateTimeOffset> CreateDailyEntries(
        DateOnly date,
        int numEntries = 4)
    {
        var shouldGenerateInterval = numEntries == CompleteEntries || Check(ProbToHaveIntervalEntry);

        var entryReturningFromInterval = numEntries / 2;

        var dateEntry = StartEntry(date);

        for (var index = 0; index < numEntries; index++)
        {
            if (index == 0)
            {
                if (Check(ProbToHaveFirstEntry))
                {
                    yield return dateEntry;
                }

                continue;
            } 

            var duration = shouldGenerateInterval && ItsIntervalReturn(index)

                ? TimeSpan.FromHours(1)
                    .Add(TimeSpan.FromMinutes(Random.Next(0, 15)))

                : TimeSpan.FromHours(Random.Next(4))
                    .Add(TimeSpan.FromMinutes(Random.Next(-30, 15)));

            dateEntry = dateEntry.Add(duration);

            yield return dateEntry;
        }

        bool ItsIntervalReturn(int currentEntry) => 
            currentEntry == entryReturningFromInterval;
    }

    private static DateTimeOffset StartEntry(DateOnly date)
    {
        return new DateTimeOffset(
            date.Year,
            date.Month,
            date.Day,
            Random.Next(6, 10),
            Random.Next(0, 30),
            0, TimeSpan.Zero);
    }
}
