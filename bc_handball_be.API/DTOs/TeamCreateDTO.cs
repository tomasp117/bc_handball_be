namespace bc_handball_be.API.DTOs
{
    public class TeamCreateDTO
    {
        public string Name { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public int ClubId { get; set; }
        public int TournamentInstanceId { get; set; }
    }
}
