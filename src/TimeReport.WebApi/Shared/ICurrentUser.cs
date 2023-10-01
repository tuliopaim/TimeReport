using System.Security.Claims;

namespace TimeReport.Shared;

public interface ICurrentUser
{
    Guid? UserId { get; }
    Guid? CompanyId { get; }
}

public class CurrentUser : ICurrentUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUser(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid? UserId => GetUser();
    public Guid? CompanyId => GetCompany();

    private Guid? GetUser()
    {
        var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue(Constants.Claims.UserId);
        if (userId == null)
        {
            return null;
        }

        return Guid.Parse(userId);
    }

    private Guid? GetCompany()
    {
        var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue(Constants.Claims.CompanyId);
        if (userId == null)
        {
            return null;
        }

        return Guid.Parse(userId);
    }
}
