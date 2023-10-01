namespace TimeReport.Shared;

public static class DateExtensions
{
    public static DateOnly ToDateOnly(this DateTime datetime) =>
        DateOnly.FromDateTime(datetime);

    public static DateOnly FirstDayOfMonth(this DateOnly date) =>
        new(
            date.Year,
            date.Month,
            1);

    public static DateOnly LastDayOfMonth(this DateOnly date)
    {
        var firstDayOfNextMonth = date.AddMonths(1).FirstDayOfMonth();

        return firstDayOfNextMonth.AddDays(-1);
    }

    public static DateTimeOffset FromDateOnly(this DateOnly date) =>
        new(date.Year, date.Month, date.Day, 0, 0, 0, TimeSpan.Zero);
}