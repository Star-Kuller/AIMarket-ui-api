using IAE.Microservice.Domain.Entities.Users;

namespace IAE.Microservice.Application.Features.Accounts.Update.Models
{
    public class UpdateValues
    {
        public string Email { get; set; }

        public bool EmailConfirmed { get; set; }

        public string Phone { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public Language Language { get; set; }

        public long CountryId { get; set; }

        public long TimezoneId { get; set; }
    }

    public class UpdateValuesWithPassword: UpdateValues
    {
        public string Password { get; set; }
    }
}