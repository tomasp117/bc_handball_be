using bc_handball_be.Core.Entities.IdentityField;

namespace bc_handball_be.Core.Entities
{
    public class Club : BaseEntity
    {
        // public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Logo { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        // Navigation properties
        public ICollection<Team> Teams { get; set; } = new List<Team>();

    }
}
