using bc_handball_be.API.DTOs.Teams;

﻿namespace bc_handball_be.API.DTOs.Clubs
{
    public class ClubDetailDTO : ClubDTO
    {
        public List<TeamDetailDTO> Teams { get; set; } = new();
    }
}
