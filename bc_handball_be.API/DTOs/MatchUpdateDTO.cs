namespace bc_handball_be.API.DTOs
{
    public class MatchUpdateDTO
    {
        public string? TimePlayed { get; set; }
        public int? HomeScore { get; set; }
        public int? AwayScore { get; set; }
        public string? State { get; set; }
        public int? SequenceNumber { get; set; }
    }
}
