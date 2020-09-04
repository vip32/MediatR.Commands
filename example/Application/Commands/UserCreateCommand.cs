namespace Application
{
    using FluentValidation.Results;
    using MediatR.Commands;

    public class UserCreateCommand : CommandBase<UserCreateCommandResponse>, IValidatedCommand
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public ValidationResult Validate() =>
            new UserCreateCommandValidator().Validate(this);
    }
}
