namespace bc_handball_be.API.DTOs
{
    public class ClubDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Logo { get; set; }
        public string Email { get; set; } = string.Empty;
        public string? Address { get; set; }
        public string? State { get; set; }
        public string? Website { get; set; }
    }
}
