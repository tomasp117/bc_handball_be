namespace bc_handball_be.API.DTOs
{
    public class TeamDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        //public int ClubId { get; set; }
        //public string ClubName { get; set; } = string.Empty;

        public ClubDTO Club { get; set; } = null!;

        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;

        public int TournamentInstanceId { get; set; }
        public int TournamentInstanceNum { get; set; } = 0;

        public List<int> GroupIds { get; set; } = new();
        public List<string> GroupNames { get; set; } = new();
    }
}
