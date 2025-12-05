namespace bc_handball_be.API.DTOs.Coaches
{
    public class CoachDetailDTO : CoachDTO
    {
        public int TeamId { get; set; }
        public string TeamName { get; set; } = string.Empty;

        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
    }
}
