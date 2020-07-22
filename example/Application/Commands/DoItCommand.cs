namespace Application
{
    using MediatR.Commands;

    public class DoItCommand : CommandBase
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }
    }
}
