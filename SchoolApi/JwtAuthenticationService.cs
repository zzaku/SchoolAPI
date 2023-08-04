using JwtApi.Models;
using Microsoft.IdentityModel.Tokens;
using SchoolApi.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace JwtApi
{
    public class JwtAuthenticationService : IJwtAuthenticationService
    {

        private readonly SchoolApiContext _dbContext;

        public JwtAuthenticationService(SchoolApiContext dbContext)
        {
            _dbContext = dbContext;
        }

        public User Authenticate(string email, string password)
        {
            return _dbContext.Users
                .FirstOrDefault(u => u.Email.ToUpper().Equals(email.ToUpper())
                && u.Password.Equals(password));
        }

        public string GenerateToken(string secret, List<Claim> claims)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(60),
                SigningCredentials = new SigningCredentials(
                    key,
                    SecurityAlgorithms.HmacSha256Signature)

            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
