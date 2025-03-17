namespace bc_handball_be.API.DTOs
{
    public class CategoryDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool VoitingOpen { get; set; }
        public int TournamentInstanceId { get; set; }
        public int TournamentInstanceNum { get; set; } = 0;
    }
}
