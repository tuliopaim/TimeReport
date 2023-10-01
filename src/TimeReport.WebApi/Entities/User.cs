using Microsoft.AspNetCore.Identity;

namespace TimeReport.Entities;

public class User : IdentityUser<Guid>, IAuditableEntity
{
    public DateTimeOffset CreatedAt { get; } = default;
    public DateTimeOffset? UpdatedAt { get; } = default;
}