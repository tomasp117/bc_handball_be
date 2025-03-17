namespace bc_handball_be.API.DTOs
{
    public class EventDTO
    {
        public int Id { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Team { get; set; } = string.Empty;
        public string Time { get; set; } = string.Empty;
        public int AuthorId { get; set; }
        public int MatchId { get; set; }
    }
}
