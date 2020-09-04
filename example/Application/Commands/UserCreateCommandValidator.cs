namespace Application
{
    using FluentValidation;

    public class UserCreateCommandValidator : AbstractValidator<UserCreateCommand>
    {
        public UserCreateCommandValidator()
        {
            this.RuleFor(c => c.FirstName).NotNull().NotEmpty();
            this.RuleFor(c => c.LastName).NotNull().NotEmpty();
        }
    }
}
