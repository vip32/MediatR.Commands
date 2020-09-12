//namespace Application.Web.Api.Controllers
//{
//    using System;
//    using System.Collections.Generic;
//    using System.Net;
//    using System.Threading.Tasks;
//    using global::Application;
//    using MediatR;
//    using MediatR.Commands;
//    using Microsoft.AspNetCore.Mvc;

//    [ApiController]
//    [Route("api/users")]
//    public class UserController : ControllerBase
//    {
//        private readonly IMediator mediator;

//        public UserController(IMediator mediator)
//        {
//            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
//        }

//        [HttpGet]
//        public async Task<IEnumerable<User>> Get()
//        {
//            return await this.mediator.Send(
//                new UserFindAllQuery()).ConfigureAwait(false);
//        }

//        [HttpGet]
//        [Route("{userId}")]
//        public async Task<User> Get(string userId)
//        {
//            return await this.mediator.Send(
//                new UserFindByIdQuery(userId)).ConfigureAwait(false);
//        }

//        [HttpPost]
//        public async Task Post(UserCreateCommand command)
//        {
//            var res = await this.mediator.Send(command).ConfigureAwait(false);
//            await this.Response.Location($"/users/{res.UserId}").ConfigureAwait(false);
//            this.Response.StatusCode = (int)HttpStatusCode.Created;
//        }

//        [HttpPut]
//        [Route("{userId}")]
//        public async Task Put(UserUpdateCommand command)
//        {
//            await this.mediator.Send(command).ConfigureAwait(false);
//        }
//    }
//}