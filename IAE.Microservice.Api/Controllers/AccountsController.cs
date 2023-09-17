using IAE.Microservice.Api.Security;
using IAE.Microservice.Application.Features.Accounts;
using IAE.Microservice.Application.Features.Accounts.Update;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using IAE.Microservice.Api.Models;
using IAE.Microservice.Domain.Entities.Users;

namespace IAE.Microservice.Api.Controllers
{
    [ApiController]
    [Route("accounts")]
    public class AccountsController : BaseController
    {
        [AllowAnonymous]
        [HttpPost("login")]
        [ProducesResponseType(typeof(TokenResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> Login([FromBody] Login.Command command)
        {
            var token = await Mediator.Send(command);
            return Ok(new TokenResponse { AccessToken = token });
        }

        [AllowAnonymous]
        [HttpPost("create/confirm")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> CreateConfirm([FromBody] CreateConfirm.Command command)
        {
            await Mediator.Send(command);
            return NoContent();
        }

        [AllowAnonymous]
        [HttpPost("register")]
        [ProducesResponseType(typeof(CreatedResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> Register([FromBody] Register.Command command)
        {
            var id = await Mediator.Send(command);
            return Ok(new CreatedResponse {Id = id});
        }

        [AllowAnonymous]
        [HttpPost("register/confirm")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> RegisterConfirm([FromBody] RegisterConfirm.Command command)
        {
            await Mediator.Send(command);
            return NoContent();
        }

        [AllowAnonymous]
        [HttpPost("register/notification")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> RegisterNotification([FromBody] RegisterNotification notification)
        {
            await Mediator.Publish(notification);
            return NoContent();
        }

        [AllowAnonymous]
        [HttpPost("password/recovery")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> RecoveryPassword([FromBody] RecoveryPassword.Command command)
        {
            await Mediator.Send(command);
            return NoContent();
        }

        [AllowAnonymous]
        [HttpPost("password/recovery/confirm")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> RecoveryPasswordConfrim([FromBody] RecoveryPasswordConfirm.Command command)
        {
            await Mediator.Send(command);
            return NoContent();
        }
        
        [HttpPost("password/change")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePassword.Command command)
        {
            await Mediator.Send(command);
            return NoContent();
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize(Roles = Role.Administrator)]
        [HttpPost("update/U1AZO6F7ONGI39V8D8WSLG072PJHRWWL7GBO")]
        [ProducesResponseType(typeof(UpdatePost.Response), StatusCodes.Status200OK)]
        public async Task<IActionResult> Update([FromBody] UpdatePost.Request command)
        {
            var response = await Mediator.Send(command);
            return response.Result.Succeeded
                ? (IActionResult) Ok(response.Result.Data)
                : StatusCode((int) response.Result.StatusCode);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize(Roles = Role.Administrator)]
        [HttpGet("update/GY8BZY7IKNGIFM0357ZHRNPRA3EU49KGNACW")]
        [ProducesResponseType(typeof(UpdateGet.Response), StatusCodes.Status200OK)]
        public async Task<IActionResult> Update([FromQuery] UpdateGet.Request command)
        {
            var response = await Mediator.Send(command);
            return response.Result.Succeeded
                ? (IActionResult) Ok(response.Result.Data)
                : StatusCode((int) response.Result.StatusCode);
        }
    }
}