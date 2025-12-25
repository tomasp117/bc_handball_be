using bc_handball_be.API.DTOs.Clubs;

namespace bc_handball_be.API.DTOs.Registrations
{
    /// <summary>
    /// DTO for creating a complete club registration (club + admin + registration).
    /// Used for public registration where the club and admin don't exist yet.
    /// </summary>
    public class ClubRegistrationCreateDTO
    {
        // Club information (to be created)
        public ClubCreateDTO Club { get; set; } = new();

        // Club admin information (to be created)
        public ClubAdminRegisterDTO ClubAdmin { get; set; } = new();

        // Registration details
        public int TournamentInstanceId { get; set; }
        public int PackageACount { get; set; }
        public int PackageBCount { get; set; }
        public List<ClubRegistrationCategoryCreateDTO> CategoryTeamCounts { get; set; } = new();
    }

    public class ClubRegistrationCategoryCreateDTO
    {
        public int CategoryId { get; set; }
        public int TeamCount { get; set; }
    }
}
