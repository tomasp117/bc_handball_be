namespace bc_handball_be.API.DTOs.Groups
{
    public class GroupDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Phase { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
    }
}