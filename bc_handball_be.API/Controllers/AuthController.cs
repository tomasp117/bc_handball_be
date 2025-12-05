using bc_handball_be.API.DTOs.Persons;
using bc_handball_be.Core.Entities;
using bc_handball_be.Core.Entities.Actors.sub;
using bc_handball_be.Core.Entities.Actors.super;
using bc_handball_be.Core.Interfaces.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace bc_handball_be.API.Controllers;

/// <summary>
/// Handles user authentication and registration.
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    /// <summary>
    /// Registers a new user (Admin only).
    /// </summary>
    /// <param name="model">Registration data including person details and role.</param>
    /// <returns>Success message if registration completes.</returns>
    /// <response code="200">User registered successfully.</response>
    /// <response code="400">If user already exists or invalid role provided.</response>
    [Authorize(Roles = "Admin")]
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterDTO model)
    {
        var person = new Person
        {
            FirstName = model.FirstName,
            LastName = model.LastName,
            Email = model.Email,
            PhoneNumber = model.PhoneNumber,
            Address = model.Address,
            DateOfBirth = model.DateOfBirth
        };

        var login = new Login
        {
            Username = model.Username,
            Person = person
        };
        login.SetPassword(model.Password); // ✅ nastavíme zde

        person.Login = login;

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

        var success = await _authService.RegisterAsync(person, login, roleEntity);
        if (!success) return BadRequest("User already exists.");

        return Ok("User registered successfully.");
    }


    /// <summary>
    /// Authenticates a user and returns a JWT token.
    /// </summary>
    /// <param name="request">Login credentials (username and password).</param>
    /// <returns>JWT token if authentication successful.</returns>
    /// <response code="200">Returns JWT token.</response>
    /// <response code="401">If credentials are invalid.</response>
    [HttpPost("login")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var token = await _authService.AuthenticateAsync(request.Username, request.Password);
        if (token == null)
        {
            _logger.LogWarning("Failed login attempt for user {Username}", request.Username);
            return Unauthorized("Invalid username or password.");
        }

        _logger.LogInformation("User {Username} logged in successfully.", request.Username);
        return Ok(new { Token = token });
    }
}

public class LoginRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}