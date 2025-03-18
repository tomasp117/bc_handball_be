using bc_handball_be.Core.Entities.Actors.sub;
using bc_handball_be.Core.Entities.IdentityField;

namespace bc_handball_be.Core.Entities
{
    public class Club : BaseEntity
    {
        // public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Logo { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;

        public ClubAdmin? ClubAdmin { get; set; } = null!;


        // Navigation properties
        public ICollection<Team> Teams { get; set; } = new List<Team>();

    }
}
