using IAE.Microservice.Common;
using IAE.Microservice.Domain.Entities.Users;
using Microsoft.Extensions.Options;
using System;
using IAE.Microservice.Application.Interfaces;
using IAE.Microservice.Application.Notifications.Models;

namespace IAE.Microservice.Application.Features.Accounts.Notifications
{
    public class MessageProvider
    {
        private class DateTimeMacrosValues
        {
            public string Date { get; set; }
            public string Time { get; set; }
            public string Timezone { get; set; }
        }

        public const string USER_MACROS = "{{USER}}";
        public const string USERNAME_MACROS = "{{USERNAME}}";
        public const string CODE_MACROS = "{{CODE}}";
        public const string LINK_MACROS = "{{LINK}}";
        public const string DATE_MACROS = "{{DATE}}";
        public const string TIME_MACROS = "{{TIME}}";
        public const string TIMEZONE_MACROS = "{{TIMEZONE}}";
        public const string OS_MACROS = "{{OS}}";
        public const string BROWSER_MACROS = "{{BROWSER}}";
        public const string IP_MACROS = "{{IP}}";
        public const string COUNTRY_MACROS = "{{COUNTRY}}";
        public const string REGION_MACROS = "{{REGION}}";
        public const string CITY_MACROS = "{{CITY}}";
        public const string ABBREVIATION_MACROS = "{{ABBREVIATION}}";

        private const string PREHEADER_MACROS = "%%preheader%%";
        private const string PREHEADER_CREATE = "";
        private const string PREHEADER_REGISTER = "";
        private const string PREHEADER_CHANGE_PASSWORD = "";
        private const string PREHEADER_RECOVERY_PASSWORD = "";

        private readonly ICurrentUser _currentUser;
        private readonly IGeoByIPSearcher _geoByIPSearcher;
        private readonly IUserAgentParser _userAgentParser;
        private readonly NotificationManagement _notificationManagement;

        public MessageProvider(
            ICurrentUser currentUser,
            IGeoByIPSearcher geoByIPSearcher,
            IUserAgentParser userAgentParser,
            IOptions<NotificationManagement> notificationManagement)
        {
            _currentUser = currentUser;
            _geoByIPSearcher = geoByIPSearcher;
            _userAgentParser = userAgentParser;
            _notificationManagement = notificationManagement.Value;
        }

        public Message GetChangePasswordMessage(User user)
        {
            var locale = _currentUser.Language == Language.Russian
                ? GeoByIPSearcherLocale.Ru
                : GeoByIPSearcherLocale.En;
            var geo = _geoByIPSearcher.Search(_currentUser.IpAddress, locale);
            var client = _userAgentParser.Parse(_currentUser.UserAgent);
            return GetChangePasswordMessage(user, _currentUser, geo, client, _notificationManagement);
        }

        public Message GetCreateMessage(User user, string code)
        {
            return GetCreateMessage(user, code, _notificationManagement);
        }

        public Message GetRegisterMessage(User user, string code)
        {
            return GetRegisterMessage(user, code, _notificationManagement);
        }

        public Message GetRecoveryPasswordMessage(User user, string code)
        {
            return GetRecoveryPasswordMessage(user, code, _notificationManagement);
        }


        private static Message GetRecoveryPasswordMessage(User user, string code, NotificationManagement management)
        {
            var abbrName = GetAbbreviationName(management);

            var subject = user.Language == Language.Russian
                ? $"Восстановление пароля в {abbrName}"
                : $"{abbrName} password recovery";

            var body = TemplateProvider.GetRecoveryPasswordTemplate(user.Language)
                .Replace(USER_MACROS, GetUserMacrosValue(user))
                .Replace(USERNAME_MACROS, user.Name)
                .Replace(CODE_MACROS, code)
                .Replace(PREHEADER_MACROS, PREHEADER_RECOVERY_PASSWORD);

            return new Message
            {
                To = user.Email,
                Subject = subject,
                Body = body
            };
        }

        private static Message GetChangePasswordMessage(User user, ICurrentUser currentUser, GeoInfo geo,
            ClientInfo client, NotificationManagement management)
        {
            var abbrName = GetAbbreviationName(management);

            var subject = user.Language == Language.Russian
                ? $"Изменение пароля в {abbrName}"
                : $"{abbrName} password change";

            var dtmv = GetDateTimeMacrosValues(currentUser);

            var body = TemplateProvider.GetChangePasswordTemplate(user.Language)
                .Replace(USER_MACROS, GetUserMacrosValue(user))
                .Replace(USERNAME_MACROS, user.Name)
                .Replace(DATE_MACROS, dtmv.Date)
                .Replace(TIME_MACROS, dtmv.Time)
                .Replace(TIMEZONE_MACROS, dtmv.Timezone)
                .Replace(IP_MACROS, currentUser.IpAddress)
                .Replace(OS_MACROS, client.OperatingSystem)
                .Replace(BROWSER_MACROS, client.Browser)
                .Replace(COUNTRY_MACROS, geo.Country)
                .Replace(REGION_MACROS, geo.Region)
                .Replace(CITY_MACROS, geo.City)
                .Replace(PREHEADER_MACROS, PREHEADER_CHANGE_PASSWORD);

            return new Message
            {
                To = user.Email,
                Subject = subject,
                Body = body
            };
        }

        private static Message GetRegisterMessage(User user, string code, NotificationManagement management)
        {
            var abbrName = GetAbbreviationName(management);

            var subject = user.Language == Language.Russian
                ? $"Подтверждение регистрации в {abbrName}"
                : $"Verify your new {abbrName} account";

            var link = $"{management.RegisterUserCallbackUrl}/{user.Id}/?code={code}";

            var body = TemplateProvider.GetRegisterTemplate(user.Language)
                .Replace(USER_MACROS, GetUserMacrosValue(user))
                .Replace(CODE_MACROS, code)
                .Replace(LINK_MACROS, link)
                .Replace(ABBREVIATION_MACROS, abbrName)
                .Replace(PREHEADER_MACROS, PREHEADER_REGISTER);

            return new Message
            {
                To = user.Email,
                Subject = subject,
                Body = body
            };
        }

        private static Message GetCreateMessage(User user, string code, NotificationManagement management)
        {
            var abbrName = GetAbbreviationName(management);

            var subject = user.Language == Language.Russian
                ? $"Добро пожаловать в {abbrName}"
                : $"Welcome to {abbrName}";

            var link = $"{management.CreateUserCallbackUrl}/{user.Id}/?code={code}";

            var body = TemplateProvider.GetCreateTemplate(user.Language)
                .Replace(USER_MACROS, GetUserMacrosValue(user))
                .Replace(LINK_MACROS, link)
                .Replace(ABBREVIATION_MACROS, abbrName)
                .Replace(PREHEADER_MACROS, PREHEADER_CREATE);

            return new Message
            {
                To = user.Email,
                Subject = subject,
                Body = body
            };
        }

        private static string GetAbbreviationName(NotificationManagement management)
        {
            var abbrName = management?.AbbreviationName;
            return string.IsNullOrWhiteSpace(abbrName) ? "Microservice" : abbrName;
        }
        
        private static string GetUserMacrosValue(User user)
        {
            if (!string.IsNullOrWhiteSpace(user.Name))
                return user.Name;

            return user.Language == Language.Russian
                ? $"сотрудник агентства {user.Name}"
                : $"{user.Name} agency employee";
        }

        private static DateTimeMacrosValues GetDateTimeMacrosValues(ICurrentUser currentUser)
        {
            var currentUserDate = SystemTime.DateTime.UtcNow;

            var date = currentUser.Language == Language.Russian
                ? currentUserDate.ToString("dd.MM.yyyy")
                : currentUserDate.ToString("MM.dd.yyyy");

            var time = currentUserDate.ToString("HH:mm:ss");

            

            return new DateTimeMacrosValues
            {
                Date = date,
                Time = time
            };
        }
    }
}