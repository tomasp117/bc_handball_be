namespace bc_handball_be.API.DTOs.Clubs
{
    /// <summary>
    /// DTO for registering a club admin during club registration.
    /// Similar to ClubAdminCreateDTO but without ClubId (club doesn't exist yet).
    /// </summary>
    public class ClubAdminRegisterDTO
    {
        // Person information
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public DateTime DateOfBirth { get; set; }

        // Login credentials
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
