using bc_handball_be.API.DTOs;
using bc_handball_be.Core.Entities.Actors.super;
using bc_handball_be.Core.Interfaces.IRepositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace bc_handball_be.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Person>>> GetUsers()
        {
            var users = await _userRepository.GetAllUsersAsync();
            return Ok(users);
        }

        // GET: api/user/{username}
        [HttpGet("{username}")]
        public async Task<ActionResult<Person>> GetUser(string username)
        {
            var login = await _userRepository.GetLoginByUsernameAsync(username);
            if (login == null)
                return NotFound();

            var user = login.Person;
            return Ok(user);
        }
    }
}
