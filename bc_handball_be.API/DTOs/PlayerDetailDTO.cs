namespace bc_handball_be.API.DTOs
{
    public class PlayerDetailDTO : PlayerDTO
    {
        public int? TeamId { get; set; }
        public string? TeamName { get; set; }

        public int CategoryId { get; set; }
        //public string CategoryName { get; set; } = string.Empty;
    }
}
