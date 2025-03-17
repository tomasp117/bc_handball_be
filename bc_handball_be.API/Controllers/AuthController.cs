using bc_handball_be.API.DTOs;
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
            if (!Enum.TryParse(model.Role, true, out UserRole role))
            {
                return BadRequest("Invalid role");
            }

            var user = new Person
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                Username = model.Username,
                Role = role
            };

            var success = await _authService.RegisterAsync(user, model.Password);
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
