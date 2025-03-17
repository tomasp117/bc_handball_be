namespace bc_handball_be.API.DTOs
{
    public class TournamentDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<TournamentInstanceDTO> Editions { get; set; } = new();
    }
}
