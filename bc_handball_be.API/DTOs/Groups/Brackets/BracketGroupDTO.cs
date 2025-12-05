namespace bc_handball_be.API.DTOs.Groups.Brackets
{
    public class BracketGroupDTO
    {
        public string Name { get; set; }
        public string? Phase { get; set; }
        public List<BracketTeamDTO> Teams { get; set; }
    }
}
