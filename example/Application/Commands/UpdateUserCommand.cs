namespace Application
{
    using MediatR;
    using MediatR.Commands;

    public class UpdateUserCommand : CommandBase
    {
        public string UserId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }
    }
}
