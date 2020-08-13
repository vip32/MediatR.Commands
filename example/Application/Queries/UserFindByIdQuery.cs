namespace Application
{
    using MediatR.Commands;

    public class UserFindByIdQuery : QueryBase<User>
    {
        public string UserId { get; set; }
    }
}
