namespace bc_handball_be.API.DTOs.Groups.Brackets
{
    public class PlaceholderGroupDTO
    {
        public string Name { get; set; } = string.Empty;
        public string Phase { get; set; } = string.Empty;
        public int? FinalGroup { get; set; }
        public List<PlaceholderTeamDTO> Teams { get; set; } = new();

    }
}
