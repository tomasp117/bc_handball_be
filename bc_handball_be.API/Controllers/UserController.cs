using bc_handball_be.Core.Entities.Actors.super;
using bc_handball_be.Core.Interfaces.IRepositories;
using Microsoft.AspNetCore.Mvc;

namespace bc_handball_be.API.Controllers;

/// <summary>
/// Handles user operations for retrieving user information.
/// </summary>
[Route("api/[controller]")]
[ApiController]
//[Authorize]
public class UserController : ControllerBase
{
    private readonly ILogger<UserController> _logger;
    private readonly IUserRepository _userRepository;

    public UserController(IUserRepository userRepository, ILogger<UserController> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    /// <summary>
    /// Gets all users.
    /// </summary>
    /// <returns>List of all users (Person entities).</returns>
    /// <response code="200">Returns the list of users.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Person>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Person>>> GetUsers()
    {
        try
        {
            var users = await _userRepository.GetAllUsersAsync();
            return Ok(users);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching all users");
            return StatusCode(500, "An error occurred while fetching users.");
        }
    }

    /// <summary>
    /// Gets a specific user by username.
    /// </summary>
    /// <param name="username">The username to search for.</param>
    /// <returns>The user (Person) details.</returns>
    /// <response code="200">Returns the user.</response>
    /// <response code="404">If user not found.</response>
    [HttpGet("{username}")]
    [ProducesResponseType(typeof(Person), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Person>> GetUser(string username)
    {
        try
        {
            var login = await _userRepository.GetLoginByUsernameAsync(username);
            if (login == null)
                return NotFound();

            var user = login.Person;
            return Ok(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching user with username {Username}", username);
            return StatusCode(500, "An error occurred while fetching the user.");
        }
    }
}