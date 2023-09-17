using FluentValidation;
using IAE.Microservice.Domain.Entities.Users;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using IAE.Microservice.Application.Exceptions;

namespace IAE.Microservice.Application.Features.Accounts.Update.Models
{
    public interface IUpdateUser
    {
        long? Id { get; set; }

        string Email { get; set; }
    }

    public class UpdateUserValidator<T> : AbstractValidator<T> where T : IUpdateUser
    {
        protected UpdateUserValidator()
        {
            RuleFor(x => x).Must(x => IsValidId(x.Id) || IsValidEmail(x.Email))
                .WithName($"{nameof(IUpdateUser)}")
                .WithMessage($"'{nameof(IUpdateUser.Id)}' or '{nameof(IUpdateUser.Email)}' must be filled in.");
        }

        private static bool IsValidId(long? id)
        {
            try
            {
                return id.HasValue && id >= -1;
            }
            catch
            {
                return false;
            }
        }

        private static bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }

    public static class UpdateUserExtensions
    {
        public static async Task<User> GetUserAsync(this IUpdateUser request, UserManager<User> userManager)
        {
            var user = (request.Id != null
                           ? await userManager.FindByIdAsync(request.Id.Value.ToString())
                           : await userManager.FindByEmailAsync(request.Email))
                       ?? throw new NotFoundException(nameof(User),
                           request.Id != null
                               ? request.Id.ToString()
                               : request.Email);
            return user;
        }
    }
}