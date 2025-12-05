
namespace bc_handball_be.API.DTOs.Coaches
{
    public class CoachCreateDTO
    {
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public DateTime DateOfBirth { get; set; }

        public string Username { get; set; } = "";
        public string Password { get; set; } = "";

        public char License { get; set; }

        public int TeamId { get; set; }
        //public int CategoryId { get; set; }
    }
}
