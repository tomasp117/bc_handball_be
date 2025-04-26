using bc_handball_be.Core.Entities.Actors.super;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using bc_handball_be.Core.Interfaces.IRepositories;
using bc_handball_be.Core.Interfaces.IServices;
using bc_handball_be.Core.Entities.Actors.sub;
using bc_handball_be.Core.Entities;
using Microsoft.Extensions.Logging;

namespace bc_handball_be.Core.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthService> _logger;

        public AuthService(IUserRepository userRepository, IConfiguration configuration, ILogger<AuthService> logger)
        {
            _userRepository = userRepository;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<string?> AuthenticateAsync(string username, string password)
        {
            var login = await _userRepository.GetLoginByUsernameAsync(username);
            if (login == null)
            {
                _logger.LogWarning("Login not found for username: {Username}", username);
                return null;
            }

            if (login == null || !login.VerifyPassword(password))
            {
                _logger.LogWarning("Invalid password for username: {Username}", username);
                return null;
            }
   

            return await GenerateJwtToken(login.Person);
        }

        public async Task<bool> RegisterAsync(Person user, Login login, object roleEntity)
        {
            if (await _userRepository.GetLoginByUsernameAsync(login.Username) != null)
                return false;

            await _userRepository.AddUserWithRoleAsync(user, login, roleEntity);

            return true;
        }


        private async Task<string> GenerateJwtToken(Person user)
        {
            var key = Encoding.ASCII.GetBytes(_configuration["JwtSettings:Secret"]);
            var tokenHandler = new JwtSecurityTokenHandler();
            var role = await GetUserRoleAsync(user); 

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Login!.Username),  // Použití Login.Username
            new Claim(ClaimTypes.Role, role) // Opraveno!
        }),
                Expires = DateTime.UtcNow.AddHours(int.Parse(_configuration["JwtSettings:ExpireHours"])),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private async Task<string> GetUserRoleAsync(Person user)
        {
            return await _userRepository.GetUserRoleAsync(user.Id);
        }
    }
}
