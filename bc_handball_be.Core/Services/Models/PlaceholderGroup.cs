namespace bc_handball_be.Core.Services.Models
{
    public class PlaceholderGroup
    {
        public string Name { get; set; } = string.Empty;
        public string Phase { get; set; } = string.Empty;
        public int? FinalGroup { get; set; }
        public List<PlaceholderTeam> Teams { get; set; } = new();

    }
}
