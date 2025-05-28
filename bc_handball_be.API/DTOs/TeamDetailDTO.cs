namespace bc_handball_be.API.DTOs
{
    public class TeamDetailDTO : TeamDTO
    {
        public List<PlayerDTO> Players { get; set; } = new();
        public CoachDTO Coach { get; set; } = new();
    }
}
