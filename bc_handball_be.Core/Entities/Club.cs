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
        public string? Address { get; set; }

        // Registration fields
        public string? ICO { get; set; } // Tax ID
        public ClubStatus Status { get; set; } = ClubStatus.Pending;

        public ClubAdmin? ClubAdmin { get; set; } = null!;

        public string? State { get; set; }
        public string? Website { get; set; }

        public bool? IsPlaceholder { get; set; } = false;

        // Navigation properties
        public ICollection<Team> Teams { get; set; } = new List<Team>();
        public ClubRegistration? ClubRegistration { get; set; }
    }

    public enum ClubStatus
    {
        Pending, // Waiting for approval
        Active, // Approved and active
        Inactive, // Denied or disabled
        Suspended, // Temporarily disabled
    }
}
