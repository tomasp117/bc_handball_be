using Microsoft.AspNetCore.Mvc;

namespace bc_handball_be.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class Controller
    {
        [HttpGet]
        public string Get()
        {
            return "Hello from the controller";
        }

    }
}
