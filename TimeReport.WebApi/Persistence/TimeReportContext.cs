using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TimeReport.Entities;

namespace TimeReport.Persistence;

public class TimeReportContext :
    IdentityDbContext<IdentityUser<Guid>, IdentityRole<Guid>, Guid>
{
    public TimeReportContext(DbContextOptions<TimeReportContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.HasDefaultSchema("time_report");

        builder.Entity<User>().ToTable("user");

        builder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        FillAuditableProperties();

        DateTimeOffsetToUtc();

        return await base.SaveChangesAsync(cancellationToken);
    }

    private void FillAuditableProperties()
    {
        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.Entity is not IAuditableEntity) continue;

            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Property(nameof(IAuditableEntity.CreatedAt)).CurrentValue = DateTimeOffset.UtcNow;
                    break;
                case EntityState.Modified:
                    entry.Property(nameof(IAuditableEntity.UpdatedAt)).CurrentValue = DateTimeOffset.UtcNow;
                    break;
            }
        }
    }

    private void DateTimeOffsetToUtc()
    {
        foreach (var entry in ChangeTracker.Entries())
        {
            foreach (var property in entry.Properties.Where(x => x.CurrentValue is not null))
            {
                if (property.Metadata.ClrType != typeof(DateTimeOffset))
                {
                    continue;
                }

                var currentValue = entry.Property(property.Metadata).CurrentValue;
                entry.Property(property.Metadata).CurrentValue = ((DateTimeOffset)currentValue!).ToUniversalTime();
            }
        }
    }

}