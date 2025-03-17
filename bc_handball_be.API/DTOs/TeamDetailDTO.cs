namespace bc_handball_be.API.DTOs
{
    public class TeamDetailDTO : TeamDTO
    {
        public List<PlayerDTO> Players { get; set; } = new();
        public List<CoachDTO> Coaches { get; set; } = new();
    }
}
