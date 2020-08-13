namespace Application
{
    using System.Collections.Generic;
    using MediatR.Commands;

    public class UserFindAllQuery : QueryBase<IEnumerable<User>>
    {
    }
}
