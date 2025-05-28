namespace bc_handball_be.API.DTOs
{
    public class ClubDetailDTO : ClubDTO
    {
        public List<TeamDetailDTO> Teams { get; set; } = new();
    }
}
