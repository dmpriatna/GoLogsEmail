using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using GoLogs.Api.Constants;
using GoLogs.Api.ViewModels;
using Microsoft.IdentityModel.Tokens;

namespace GoLogs.Api.Application.Internals
{
    public static class AuthHelper
    {
        public static AuthViewModel JWTAuth(PersonViewModel person)
        {
            var claims = new List<Claim> 
            { 
                new Claim(ClaimTypes.Email, person.Email),
                new Claim(ClaimTypes.Name, person.FullName),
                new Claim(ClaimTypes.NameIdentifier, person.Id.ToString()),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, person.Company == null ? "" : person.Company.Type)
            };

            //var roles = await _userManager.GetRolesAsync(user);

            //claims.AddRange(roles.Select(role => new Claim(ClaimsIdentity.DefaultRoleClaimType, role)));

            //var roleClaims = roles.Select(r => new Claim(ClaimTypes.Role, r));
            //claims.AddRange(roleClaims);

            var issued = DateTime.Now;
            var expires = issued.AddDays(1);
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Constant.GoLogsAuthKey));
            var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
            var tokenOptions = new JwtSecurityToken(
                claims: claims,
                expires: expires,
                signingCredentials: signinCredentials
            );
            var tokenString = new JwtSecurityTokenHandler().WriteToken(tokenOptions);

            var authViewModel = new AuthViewModel()
            {
                AccessToken = tokenString,
                Issued = issued,
                Expires = expires,
            };

            return authViewModel;
        }
    }
}
