﻿namespace Application
{
    using MediatR.Commands;

    public class CreateUserCommand : CommandBase<CreateUserCommandResponse>
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }
    }
}
