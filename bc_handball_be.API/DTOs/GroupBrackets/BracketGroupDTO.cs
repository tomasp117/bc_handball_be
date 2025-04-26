namespace bc_handball_be.API.DTOs.GroupBrackets
{
    public class BracketGroupDTO
    {
        public string Name { get; set; }
        public List<BracketTeamDTO> Teams { get; set; }
    }
}
