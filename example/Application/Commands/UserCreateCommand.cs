namespace Application
{
    using MediatR.Commands;

    public class UserCreateCommand : CommandBase<UserCreateCommandResponse>
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }
    }
}
