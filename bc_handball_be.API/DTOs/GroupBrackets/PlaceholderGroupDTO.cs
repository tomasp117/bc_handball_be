namespace bc_handball_be.API.DTOs.GroupBrackets
{
    public class PlaceholderGroupDTO
    {
        public string Name { get; set; } = string.Empty;
        public string Phase { get; set; } = string.Empty;
        public List<PlaceholderTeamDTO> Teams { get; set; } = new();

    }
}
