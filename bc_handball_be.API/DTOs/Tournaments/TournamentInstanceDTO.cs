using bc_handball_be.API.DTOs.Teams;
using bc_handball_be.API.DTOs.Categories;

﻿namespace bc_handball_be.API.DTOs.Tournaments
{
    public class TournamentInstanceDTO
    {
        public int Id { get; set; }
        public int EditionNumber { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int TournamentId { get; set; }
        public string TournamentName { get; set; } = string.Empty;

        public List<TeamDTO> Teams { get; set; } = new();
        public List<CategoryDTO> Categories { get; set; } = new();
    }
}