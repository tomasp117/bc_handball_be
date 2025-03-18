using bc_handball_be.API.DTOs;
using bc_handball_be.Core.Entities.Actors.sub;
using bc_handball_be.Core.Entities;
using bc_handball_be.Core.Entities.Actors.super;
using bc_handball_be.Core.Interfaces.IServices;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace bc_handball_be.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO model)
        {
            // Vytvoření osoby
            var person = new Person
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                Address = model.Address,
                DateOfBirth = model.DateOfBirth
            };

            // Vytvoření loginu
            var login = new Login
            {
                Username = model.Username,
                Person = person
            };
            login.SetPassword(model.Password);
            person.Login = login;

            // Vytvoření role jako samostatné entity
            object roleEntity = model.Role.ToLower() switch
            {
                "admin" => new Admin { Person = person },
                "coach" => new Coach { Person = person },
                "referee" => new Referee { Person = person },
                "recorder" => new Recorder { Person = person },
                "player" => new Player { Person = person },
                "clubadmin" => new ClubAdmin { Person = person },
                _ => null
            };

            if (roleEntity == null)
                return BadRequest("Invalid role");

            // Uložení uživatele včetně role
            var success = await _authService.RegisterAsync(person, model.Username, model.Password, roleEntity);
            if (!success) return BadRequest("User already exists.");

            return Ok("User registered successfully.");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var token = await _authService.AuthenticateAsync(request.Username, request.Password);
            if (token == null) return Unauthorized("Invalid username or password.");
            return Ok(new { Token = token });
        }
    }
    public class LoginRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
