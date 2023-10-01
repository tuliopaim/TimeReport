using System.Diagnostics;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using NpgsqlTypes;
using TimeReport.Generator;

var services = new ServiceCollection();

services.AddNpgsqlDataSource(
    "Server=localhost;Database=TimeReport; UID=postgres; PWD=senhaS3creta; Include Error Detail=true");

services.AddSingleton<Repository>();

await CreateEmployees(services.BuildServiceProvider());

return;

static async Task CreateEmployees(ServiceProvider buildServiceProvider)
{
    var repository = buildServiceProvider.GetRequiredService<Repository>();

    var companyId = new Guid("51934247-1B34-4443-8636-BFC59C86E411");

    const int total = 5000;
    const int chunkSize = 50;

    var sw = Stopwatch.StartNew();

    var httpClient = new HttpClient
    {
        BaseAddress = new Uri("http://localhost:5141/api/employee")
    };

    for (var i = 0; i < total; i += chunkSize)
    {
        var employeeChunk = EmployeeGenerator.CreateEmployees(companyId, chunkSize);

        await Parallel.ForEachAsync(
            employeeChunk,
            new ParallelOptions
            {
                MaxDegreeOfParallelism = 8,
            }, async (employee, ct) =>
            {
                var response = await httpClient.PostAsJsonAsync("", employee, ct);

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Error: {response.StatusCode}");
                    return;
                }

                var id = await response.Content.ReadFromJsonAsync<string>();
                if (!Guid.TryParse(id, out var guid))
                {
                    Console.WriteLine($"Error on employee: " + id);
                    return;
                }

                employee.Id = guid;
            });

        await Parallel.ForEachAsync(
            employeeChunk.Where(x => x.Id != Guid.Empty),
            new ParallelOptions
            {
                MaxDegreeOfParallelism = 2,
            }, async (employee, ct) =>
            {
                await repository.Insert(employee.Id, employee.Entries!, ct);
            });

        Console.WriteLine($"Chunk inserted in {TimeSpan.FromMilliseconds(sw.ElapsedMilliseconds).TotalSeconds} seconds: {i} users inserted!");
        sw.Restart();
    }
}

public class Repository
{
    private readonly NpgsqlDataSource _connection;

    public Repository(NpgsqlDataSource connection)
    {
        _connection = connection;
    }

    public async Task Insert(Guid employeeId, IEnumerable<DateTimeOffset> entries, CancellationToken cancellationToken)
    {
        var cmd = _connection.CreateCommand();

        var entryList = entries.ToList();
        var ids = entryList.Select(x => Guid.NewGuid()).ToArray();
        var times = entryList.Select(e => e.ToUniversalTime()).ToArray();
        var createdDates = Enumerable.Repeat(DateTimeOffset.UtcNow, entryList.Count).ToArray();
        var employeeIds = Enumerable.Repeat(employeeId, entryList.Count).ToArray();

        cmd.CommandText =
            """
            insert into "time_report"."time-entry" ("Id", "Time", "CreatedAt", "EmployeeId")
                      select unnest(@Ids::uuid[]), unnest(@Times::timestamptz[]), unnest(@CreatedDates::timestamptz[]), unnest(@EmployeeIds::uuid[])
            """;

        cmd.Parameters.AddWithValue("Ids", NpgsqlDbType.Array | NpgsqlDbType.Uuid, ids);
        cmd.Parameters.AddWithValue("Times", NpgsqlDbType.Array | NpgsqlDbType.TimestampTz, times);
        cmd.Parameters.AddWithValue("CreatedDates", NpgsqlDbType.Array | NpgsqlDbType.TimestampTz, createdDates);
        cmd.Parameters.AddWithValue("EmployeeIds", NpgsqlDbType.Array | NpgsqlDbType.Uuid, employeeIds);

        await cmd.ExecuteNonQueryAsync(cancellationToken);
    }
}