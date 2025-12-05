using bc_handball_be.Core.Interfaces.IServices;
using Microsoft.AspNetCore.Mvc;

namespace bc_handball_be.API.Controllers
{
    /// <summary>
    /// Handles email sending operations for testing and notifications.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class EmailController : ControllerBase
    {
        private readonly IEmailService _emailService;

        public EmailController(IEmailService emailService)
        {
            _emailService = emailService;
        }

        /// <summary>
        /// Sends a test email to verify email configuration.
        /// </summary>
        /// <param name="email">The recipient email address.</param>
        /// <returns>Success message if email sent successfully.</returns>
        /// <response code="200">Email sent successfully.</response>
        /// <response code="400">If email address is invalid or sending fails.</response>
        [HttpPost("test")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SendTestEmail([FromQuery] string email)
        {
            await _emailService.SendEmailAsync(
                email,
                "Testovací email 🚀",
                "<h1>Funguje! ✅</h1><p>Gratulujeme, email byl úspěšně odeslán!</p>"
            );
            return Ok("Email byl odeslán na " + email);
        }
    }
}
