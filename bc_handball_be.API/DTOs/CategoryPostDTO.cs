namespace bc_handball_be.API.DTOs
{
    public class CategoryPostDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int TournamentInstanceId { get; set; }
    }
}
