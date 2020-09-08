namespace Application
{
    using FluentValidation.Results;
    using MediatR.Commands;

    public class UserCreateCommand : CommandBase<UserCreateCommandResponse>, IValidated
    {
        public UserCreateCommand(string firstName, string lastName)
        {
            this.FirstName = firstName;
            this.LastName = lastName;
        }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public ValidationResult Validate() =>
            new UserCreateCommandValidator().Validate(this);
    }
}
