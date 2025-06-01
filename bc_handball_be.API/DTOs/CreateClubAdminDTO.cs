namespace bc_handball_be.API.DTOs
{
    public class CreateClubAdminDTO
    {
        public int ClubId { get; set; }
        public string FirstName { get; set; } 
        public string LastName { get; set; } 
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Username { get; set; } 
        public string Password { get; set; } 
    }

}
