using IAE.Microservice.Application.Interfaces;
using IdentityModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using IAE.Microservice.Domain.Entities.Users;

namespace IAE.Microservice.Api.Security
{
    public class CurrentUserMiddleware : IMiddleware
    {
        private readonly ICurrentUser _currentUser;
        private readonly UserManager<User> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserMiddleware(
            ICurrentUser currentUser, UserManager<User> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _currentUser = currentUser;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            if (!context.User.Identity.IsAuthenticated)
            {
                await next(context);
                return;
            }

            var userId = long.Parse(context.User.FindFirst(JwtClaimTypes.Id)?.Value);

            var user = await _userManager
                .Users
                .FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return;
            }

            _currentUser.Id = user.Id;
            _currentUser.Email = user.Email;
            _currentUser.Role = context.User.FindFirst(JwtClaimTypes.Role)?.Value;
            _currentUser.Language = user.Language;
            _currentUser.IpAddress = _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.MapToIPv4()?.ToString();
            _currentUser.UserAgent = GetHeaderValue("User-Agent");
            await next(context);
        }

        private string GetHeaderValue(string headerName)
        {
            var headers = _httpContextAccessor.HttpContext?.Request?.Headers;
            if (headers != null && headers.ContainsKey(headerName))
                return headers["User-Agent"].FirstOrDefault();

            return string.Empty;
        }
    }
}
