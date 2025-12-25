namespace bc_handball_be.API.DTOs.Clubs
{
    /// <summary>
    /// DTO for creating a new club during registration.
    /// </summary>
    public class ClubCreateDTO
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Address { get; set; }
        public string? ICO { get; set; }  // Tax ID
        public string? State { get; set; }
        public string? Website { get; set; }
        public string? Logo { get; set; }
    }
}
