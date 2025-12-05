namespace bc_handball_be.API.DTOs.Lineups
{
    public class LineupDTO
    {
        public int Id { get; set; }
        public int MatchId { get; set; }
        public int TeamId { get; set; }
        public List<LineupPlayerDTO> Players { get; set; } = new List<LineupPlayerDTO>();
    }
}
