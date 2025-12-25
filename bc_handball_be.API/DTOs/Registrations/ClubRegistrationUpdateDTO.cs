namespace bc_handball_be.API.DTOs.Registrations
{
    public class ClubRegistrationUpdateDTO
    {
        public int PackageACount { get; set; }
        public int PackageBCount { get; set; }
        public List<ClubRegistrationCategoryCreateDTO> CategoryTeamCounts { get; set; } = new();
    }
}
