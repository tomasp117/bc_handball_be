namespace bc_handball_be.API.DTOs
{
    public class ClubDetailDTO : ClubDTO
    {
        public List<TeamDTO> Teams { get; set; } = new();
    }
}
