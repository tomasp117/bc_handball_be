namespace bc_handball_be.API.DTOs.Teams
{
    public class TeamImportCsvDTO
    {
        public string Name { get; set; } = string.Empty;   
        public string ClubName { get; set; } = string.Empty;       
        public string CategoryName { get; set; } = string.Empty;  
        public int TournamentInstanceId { get; set; }
    }
}
