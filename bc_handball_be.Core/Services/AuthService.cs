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

namespace bc_handball_be.Core.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;

        public AuthService(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }

        public async Task<string?> AuthenticateAsync(string username, string password)
        {
            var login = await _userRepository.GetLoginByUsernameAsync(username);
            if (login == null || !login.VerifyPassword(password))
                return null;

            return await GenerateJwtToken(login.Person);
        }

        public async Task<bool> RegisterAsync(Person user, string username, string password, object roleEntity)
        {
            // Ověření, zda už uživatel s tímto username existuje
            if (await _userRepository.GetLoginByUsernameAsync(username) != null)
                return false;

            // Vytvoření loginu
            var login = new Login
            {
                Username = username,
                Person = user
            };
            login.SetPassword(password);
            user.Login = login;

            // Přidání uživatele, loginu a role přes repository
            await _userRepository.AddUserWithRoleAsync(user, login.Username, login.Password, roleEntity);

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
