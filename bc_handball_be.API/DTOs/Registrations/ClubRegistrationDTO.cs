namespace bc_handball_be.API.DTOs.Registrations
{
    public class ClubRegistrationDTO
    {
        public int Id { get; set; }
        public int PackageACount { get; set; }
        public int PackageBCount { get; set; }
        public float CalculatedFee { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime SubmittedDate { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public DateTime? DeniedDate { get; set; }
        public string? DenialReason { get; set; }
        public int ClubId { get; set; }
        public string ClubName { get; set; } = string.Empty;
        public int TournamentInstanceId { get; set; }
        public int TournamentInstanceNumber { get; set; }
        public List<ClubRegistrationCategoryDTO> CategoryTeamCounts { get; set; } = new();
    }
}
