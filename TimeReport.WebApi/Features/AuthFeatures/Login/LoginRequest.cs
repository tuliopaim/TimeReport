namespace TimeReport.Features.AuthFeatures.Login;

public record LoginRequest(
    string Username,
    string Password);