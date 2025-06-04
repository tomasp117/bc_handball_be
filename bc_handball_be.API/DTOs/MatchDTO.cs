namespace bc_handball_be.API.DTOs
{
    public class MatchDTO
    {
        public int Id { get; set; }
        public DateTime Time { get; set; }
        public string TimePlayed { get; set; } = string.Empty;
        public string Playground { get; set; } = string.Empty;
        public int? HomeScore { get; set; }
        public int? AwayScore { get; set; }
        //public string Score { get; set; } = "0:0";
        public string State { get; set; } = "None";

        public TeamDetailDTO HomeTeam { get; set; } = null!;
        public int? HomeTeamId { get; set; }
        public TeamDetailDTO AwayTeam { get; set; } = null!;
        public int? AwayTeamId { get; set; }
        public GroupDTO Group { get; set; } = null!;
        public int? GroupId { get; set; }

        public List<LineupDTO>? Lineups { get; set; } = new List<LineupDTO>();
    }
}