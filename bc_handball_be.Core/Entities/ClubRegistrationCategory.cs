using bc_handball_be.Core.Entities.IdentityField;

namespace bc_handball_be.Core.Entities
{
    public class ClubRegistrationCategory : BaseEntity
    {
        // public int Id { get; set; }

        public int TeamCount { get; set; } = 0;

        // Foreign keys
        public int ClubRegistrationId { get; set; }
        public ClubRegistration ClubRegistration { get; set; } = null!;

        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;
    }
}
