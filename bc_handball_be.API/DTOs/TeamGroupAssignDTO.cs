namespace bc_handball_be.API.DTOs
{
    public class TeamGroupAssignDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int CategoryId { get; set; }

        public int? Strength { get; set; } = 1;
        public bool? IsGirls { get; set; } = false;
    }
}
