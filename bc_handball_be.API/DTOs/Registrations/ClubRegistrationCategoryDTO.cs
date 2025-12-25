namespace bc_handball_be.API.DTOs.Registrations
{
    public class ClubRegistrationCategoryDTO
    {
        public int Id { get; set; }
        public int TeamCount { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
    }
}
