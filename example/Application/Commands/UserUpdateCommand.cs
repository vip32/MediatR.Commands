namespace Application
{
    using MediatR.Commands;

    public class UserUpdateCommand : CommandBase
    {
        public UserUpdateCommand()
        {
        }

        public UserUpdateCommand(string firstName, string lastName)
        {
            this.FirstName = firstName;
            this.LastName = lastName;
        }

        public string Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }
    }
}
