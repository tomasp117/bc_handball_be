namespace bc_handball_be.API.DTOs
{
    public class GroupDetailDTO : GroupDTO
    {
        public List<TeamDTO> Teams { get; set; } = new();
        public List<MatchDTO> Matches { get; set; } = new();
    }
}
