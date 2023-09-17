using IAE.Microservice.Api.Security;
using IAE.Microservice.Application.Common;
using IAE.Microservice.Application.Features.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using IAE.Microservice.Domain.Entities.Users;

namespace IAE.Microservice.Api.Controllers.Users
{
    [ApiController]
    [Route("users")]
    public class UsersController : BaseController
    {
        [HttpPut("{id}/status")]
        [Authorize(Roles = Role.Administrator)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> ChangeStatus(long id, [FromBody] ChangeStatus.Command command)
        {
            if (id != command.Id)
                return BadRequest();

            await Mediator.Send(command);

            return NoContent();
        }

        [HttpPut("{id}/email_confirmed")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> ChangeEmailConfirmed(long id, [FromBody] ChangeEmailConfirmed.Command command)
        {
            if (id != command.Id)
                return BadRequest();

            await Mediator.Send(command);

            return NoContent();
        }
    }
}