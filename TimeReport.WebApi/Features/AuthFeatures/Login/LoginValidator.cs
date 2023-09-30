using FluentValidation;

namespace TimeReport.Features.AuthFeatures.Login;

public class LoginValidator : Validator<LoginRequest>
{
    public LoginValidator()
    {
        RuleFor(x => x.Username).MaximumLength(16).NotEmpty();
        RuleFor(x => x.Password).MinimumLength(3).NotEmpty();
    }
}