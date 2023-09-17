using System.Threading.Tasks;
using IAE.Microservice.Application.Features.Users;
using IAE.Microservice.Domain.Entities.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IAE.Microservice.Api.Controllers
{
    [ApiController]
    [Route("users")]
    public class UsersController : BaseController
    {
        
        /// <summary>
        /// Изменить статус пользователя
        /// </summary>
        /// <param name="id">id пользователя</param>
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

        /// <summary>
        /// Подтвердить почту пользователя
        /// </summary>
        /// <param name="id">id пользователь</param>
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