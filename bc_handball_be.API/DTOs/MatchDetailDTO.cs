namespace bc_handball_be.API.DTOs
{
    public class MatchDetailDTO : MatchDTO
    {
        public List<EventDTO> Events { get; set; } = new();
        public int? MainRefereeId { get; set; }
        public string? MainRefereeName { get; set; }
        public int? AssistantRefereeId { get; set; }
        public string? AssistantRefereeName { get; set; }
    }
}
