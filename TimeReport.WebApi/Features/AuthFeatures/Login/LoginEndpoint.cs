using FastEndpoints.Security;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using TimeReport.Entities;

namespace TimeReport.Features.AuthFeatures.Login;

public record LoginResponse(
    string Token,
    DateTimeOffset ValidTo);

public class LoginEndpoint : Endpoint<LoginRequest, Results<Ok<LoginResponse>, ProblemDetails>>
{
    private readonly SignInManager<User> _signInManager;
    private readonly IConfiguration _configuration;

    public LoginEndpoint(
        SignInManager<User> signInManager,
        IConfiguration configuration)
    {
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
        var user = await _signInManager.UserManager.FindByNameAsync(req.Username);

        if (user is null)
        {
            AddError("User not found", "UserNotFound");
            return new ProblemDetails(ValidationFailures);
        }

        var loginResult = await _signInManager.CheckPasswordSignInAsync(user, req.Password, false);
        if (!loginResult.Succeeded)
        {
            AddError("Invalid credentials", "InvalidCredentials");
            return new ProblemDetails(ValidationFailures);
        }

        var validTo = DateTimeOffset.UtcNow.AddHours(1);
        var jwtToken = JWTBearer.CreateToken(
            _configuration["Jwt:Key"]!,
            u =>
            {
                u["UserID"] = user.Id;
                u["UserName"] = req.Username;
            },
            "TimeReport",
            "TimeReport",
            validTo.DateTime);

        return TypedResults.Ok(new LoginResponse(jwtToken, validTo));
    }
}