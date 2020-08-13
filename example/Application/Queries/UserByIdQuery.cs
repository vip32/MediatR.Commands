namespace Application
{
    using MediatR.Commands;

    public class UserByIdQuery : QueryBase<User>
    {
        public string UserId { get; set; }
    }
}
