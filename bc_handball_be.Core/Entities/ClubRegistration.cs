using bc_handball_be.Core.Entities.IdentityField;

namespace bc_handball_be.Core.Entities
{
    public class ClubRegistration : BaseEntity
    {
        // public int Id { get; set; }

        // Accommodation & catering packages
        public int PackageACount { get; set; } = 0; // Thu-Sun (3 nights)
        public int PackageBCount { get; set; } = 0; // Fri-Sun (2 nights)

        // Fee calculation
        public float CalculatedFee { get; set; } = 0;

        // Status tracking
        public RegistrationStatus Status { get; set; } = RegistrationStatus.Pending;
        public DateTime SubmittedDate { get; set; } = DateTime.UtcNow;
        public DateTime? ApprovedDate { get; set; }
        public DateTime? DeniedDate { get; set; }
        public string? DenialReason { get; set; }

        // Foreign keys
        public int ClubId { get; set; }
        public Club Club { get; set; } = null!;

        public int TournamentInstanceId { get; set; }
        public TournamentInstance TournamentInstance { get; set; } = null!;

        // Navigation properties
        public ICollection<ClubRegistrationCategory> CategoryTeamCounts { get; set; } =
            new List<ClubRegistrationCategory>();
    }

    public enum RegistrationStatus
    {
        Pending,
        Approved,
        Denied,
    }
}
