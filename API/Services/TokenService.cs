using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using Domain;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Configuration;

namespace API.Services
{
    public class TokenService
    {
        private readonly IConfiguration _config;
        public TokenService(IConfiguration config)
        {
            _config = config;
        }
        public string CreateToken(AppUser user)
        {
            //claims will be used to build authentication token
            //These clai,s are things to think about when authenticsating someone, including name , id, and email
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email),
            };

            //This key stays only on the server
            //It can only be decoded by a single key
            //The text inside GetBytes needs to be at least 2 chars
            //key is for the token
            //The key is in appsettingd.Developmeny.json. The _config grabs this information
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["TokenKey"]));

            //credentials for token
            //Secure algorthm Hmac... is used to encrypt the key
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

            //3 fields to build Jwt token
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(7),
                SigningCredentials = creds
            };

            //Takes all the information and makes authentication token using JwtTokenHandler
            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}