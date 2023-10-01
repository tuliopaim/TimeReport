using FluentValidation;

namespace TimeReport.Features.EmployeeFeatures.Create;

public class CreateEmployeeValidator : Validator<CreateEmployeeRequest>
{
    public CreateEmployeeValidator()
    {
        RuleFor(x => x.Username).MaximumLength(16).NotEmpty();
        RuleFor(x => x.FirstName).MaximumLength(16).NotEmpty();
        RuleFor(x => x.LastName).MaximumLength(32);
        RuleFor(x => x.Password).MinimumLength(3).NotEmpty();
        RuleFor(x => x.CompanyId).NotEmpty();
        RuleFor(x => x.Type).IsInEnum();
        RuleFor(x => x.DailyHours).GreaterThan((short)0);
    }
}