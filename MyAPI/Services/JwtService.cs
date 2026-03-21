using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using MyAPI.Models.Entities;

namespace MyAPI.Services
{
    public class JwtService
    {
        private readonly IConfiguration _config;

        public JwtService(IConfiguration config)
        {
            _config = config;
        }

        public string GenerateToken(User user)
        {
            var secret = Environment.GetEnvironmentVariable("JWT_SECRET_KEY");

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(secret!)
            );

            var creds = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256
            );

            var claims = new List<Claim>
            {
                new Claim("id", user.Id.ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), 
                new Claim("sub", user.Username),
                new Claim("username", user.Username),
                new Claim("jti", Guid.NewGuid().ToString())
            };

            foreach (var role in user.UserRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role.Role.Name));

                foreach (var permission in role.Role.RolePermissions)
                {
                    claims.Add(
                        new Claim("permission", permission.Permission.Name)
                    );
                }
            }

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}