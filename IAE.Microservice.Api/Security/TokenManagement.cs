﻿using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace IAE.Microservice.Api.Security
{
    public class TokenManagement
    {
        /// <summary>
        /// Secret key for encryption tokens
        /// </summary>
        public string Secret { get; set; }

        public SymmetricSecurityKey SecurityKey => new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Secret));

        public string Issuer { get; set; }

        public string Audience { get; set; }

        /// <summary>
        /// Access token lifetime in days
        /// </summary>        
        public int Lifetime { get; set; }
    }
}
