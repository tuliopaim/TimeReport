using FastEndpoints.Security;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TimeReport.Entities;
using TimeReport.Persistence;
using TimeReport.Shared;

namespace TimeReport.Features.AuthFeatures.Login;

public record LoginRequest(
    string UserName,
    string Password);

public class LoginValidator : Validator<LoginRequest>
{
    public LoginValidator()
    {
        RuleFor(x => x.UserName).MaximumLength(16).NotEmpty();
        RuleFor(x => x.Password).MinimumLength(3).NotEmpty();
    }
}

public record LoginResponse(
    string Token,
    DateTimeOffset ValidTo);

public class LoginEndpoint : Endpoint<LoginRequest, Results<Ok<LoginResponse>, ProblemDetails>>
{
    private readonly TimeReportContext _context;
    private readonly SignInManager<User> _signInManager;
    private readonly IConfiguration _configuration;

    public LoginEndpoint(
        TimeReportContext context,
        SignInManager<User> signInManager,
        IConfiguration configuration)
    {
        _context = context;
        _signInManager = signInManager;
        _configuration = configuration;
    }

    public override void Configure()
    {
        Post("/api/login");
        AllowAnonymous();
    }

    public override async Task<Results<Ok<LoginResponse>, ProblemDetails>> ExecuteAsync(
        LoginRequest req,
        CancellationToken ct)
    {
        var userInfo = await GetUserInfo(req, ct);

        if (userInfo is null)
        {
            AddError("User not found", "UserNotFound");
            return new ProblemDetails(ValidationFailures);
        }

        var loginResult = await _signInManager
            .PasswordSignInAsync(req.UserName, req.Password, false, false);

       if (!loginResult.Succeeded)
        {
            AddError("Invalid credentials", "InvalidCredentials");
            return new ProblemDetails(ValidationFailures);
        }

        var validTo = DateTimeOffset.UtcNow.AddHours(1);

        var jwtToken = CreateToken(req, userInfo, validTo);

        return TypedResults.Ok(new LoginResponse(jwtToken, validTo));
    }

    private string CreateToken(
        LoginRequest req,
        UserInfo userInfo,
        DateTimeOffset validTo)
    {
        var jwtToken = JWTBearer.CreateToken(
            _configuration["Jwt:Key"]!,
            u =>
            {
                u.Roles.Add(userInfo.Type.ToString());
                u[Constants.Claims.UserName] = req.UserName;
                u[Constants.Claims.UserId] = userInfo.UserId.ToString();
                u[Constants.Claims.CompanyId] = userInfo.CompanyId.ToString();
            },
            "TimeReport",
            "TimeReport",
            validTo.DateTime);
        return jwtToken;
    }

    private async Task<UserInfo?> GetUserInfo(LoginRequest req, CancellationToken ct)
    {
        return await _context
            .Employees
            .Where(u => u.User!.UserName == req.UserName)
            .Select(x => new UserInfo
            {
                UserId = x.UserId,
                CompanyId = x.CompanyId,
                Type = x.Type
            })
            .FirstOrDefaultAsync(ct);
    }

    private class UserInfo
    {
        public Guid UserId { get; set; }
        public Guid CompanyId { get; set; }
        public EmployeeType Type { get; set; }
    }
}