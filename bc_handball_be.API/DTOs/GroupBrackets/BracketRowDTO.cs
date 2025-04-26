namespace bc_handball_be.API.DTOs.GroupBrackets
{
    public class BracketRowDTO
    {
        public string Name { get; set; }
        public List<BracketGroupDTO> Groups { get; set; }
    }
}
