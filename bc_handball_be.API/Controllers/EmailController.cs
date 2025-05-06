using bc_handball_be.Core.Interfaces.IServices;
using Microsoft.AspNetCore.Mvc;

namespace bc_handball_be.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmailController : ControllerBase
    {
        private readonly IEmailService _emailService;

        public EmailController(IEmailService emailService)
        {
            _emailService = emailService;
        }

        [HttpPost("test")]
        public async Task<IActionResult> SendTestEmail([FromQuery] string email)
        {
            try
            {
                await _emailService.SendEmailAsync(
                    email,
                    "Testovací email 🚀",
                    "<h1>Funguje! ✅</h1><p>Gratulujeme, email byl úspěšně odeslán!</p>"
                );
                return Ok("Email byl odeslán na " + email);
            }
            catch (Exception ex)
            {
                return BadRequest("Chyba při odesílání emailu: " + ex.Message);
            }
        }
    }
}
