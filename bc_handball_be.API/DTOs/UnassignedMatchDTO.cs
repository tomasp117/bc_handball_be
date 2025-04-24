namespace bc_handball_be.API.DTOs
{
    public class UnassignedMatchDTO
    {
        public int HomeTeamId { get; set; }
        public string HomeTeamName { get; set; } = "";

        public int AwayTeamId { get; set; }
        public string AwayTeamName { get; set; } = "";

        public int GroupId { get; set; }
        public string GroupName { get; set; } = "";
    }
}
