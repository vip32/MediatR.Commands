namespace Application
{
    using MediatR.Commands;

    public class UserFindByIdQuery : QueryBase<User>
    {
        public UserFindByIdQuery()
        {
        }

        public UserFindByIdQuery(string userId)
        {
            this.UserId = userId;
        }

        public string UserId { get; set; }
    }
}
