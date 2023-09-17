using IAE.Microservice.Application.Interfaces;
using IAE.Microservice.Application.Tokens.Models;
using IdentityModel;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace IAE.Microservice.Api.Security
{
    public class JwtTokenProvider : ITokenProvider
    {
        private readonly TokenManagement _tokenManagement;

        public JwtTokenProvider(IOptions<TokenManagement> tokenManagement)
        {
            _tokenManagement = tokenManagement.Value;
        }

        public string GetToken(UserClaims userClaims)
        {
            var jwtToken = new JwtSecurityToken(
                issuer: _tokenManagement.Issuer,
                audience: _tokenManagement.Audience,
                claims: new List<Claim>
                {
                    new Claim(JwtClaimTypes.Id, userClaims.Id.ToString()),
                    new Claim(JwtClaimTypes.Role, userClaims.Role),
                },
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddDays(_tokenManagement.Lifetime),
                signingCredentials: new SigningCredentials(_tokenManagement.SecurityKey, SecurityAlgorithms.HmacSha256)
            );
            var token = new JwtSecurityTokenHandler().WriteToken(jwtToken);
            return token;
        }
    }
}
