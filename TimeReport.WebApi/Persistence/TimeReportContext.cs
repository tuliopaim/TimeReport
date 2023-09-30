using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TimeReport.Entities;

namespace TimeReport.Persistence;

public class TimeReportContext :
    IdentityDbContext<User, IdentityRole<Guid>, Guid>
{
    public TimeReportContext(DbContextOptions<TimeReportContext> options)
        : base(options)
    {
    }

    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<Company> Companies => Set<Company>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.HasDefaultSchema("time_report");

        builder.Entity<User>(b =>
        {
            b.ToTable("user");

            b.Property(e => e.CreatedAt).IsRequired();
            b.Property(e => e.CreatedAt);
        });

        builder.Entity<Employee>(b =>
        {
            b.ToTable("employee");

            b.HasKey(e => e.Id);

            b.Property(e => e.FirstName)
                .IsRequired()
                .HasMaxLength(16);

            b.Property(e => e.LastName).HasMaxLength(32);

            b.Property(e => e.Type).IsRequired();

            b.Property(e => e.CreatedAt).IsRequired();
            b.Property(e => e.CreatedAt);

            b.HasOne(x => x.User).WithOne().HasForeignKey<Employee>(e => e.UserId);
            b.HasOne<Company>().WithMany().HasForeignKey(e => e.CompanyId);
        });

        builder.Entity<Company>(b =>
        {
            b.ToTable("company");

            b.HasKey(e => e.Id);

            b.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(32);

            b.Property(e => e.CreatedAt).IsRequired();
            b.Property(e => e.CreatedAt);

            b.HasData(
                new Company("Company 1")
                {
                    Id = Guid.Parse("51934247-1B34-4443-8636-BFC59C86E411"),
                },
                new Company("Company 2")
                {
                    Id = Guid.Parse("74CEE1A7-7C55-4148-B6ED-FAD5DB38459B"),
                });
        });

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