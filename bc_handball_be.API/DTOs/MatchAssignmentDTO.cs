namespace bc_handball_be.API.DTOs
{
    public class MatchAssignmentDTO
    {
        public int MatchId { get; set; }
        public int HomeTeamId { get; set; }
        public int AwayTeamId { get; set; }
        public int GroupId { get; set; }
        public int? SequenceNumber { get; set; }
    }
}
