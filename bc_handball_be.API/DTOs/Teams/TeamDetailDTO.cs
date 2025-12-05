using bc_handball_be.API.DTOs.Players;
using bc_handball_be.API.DTOs.Coaches;

﻿namespace bc_handball_be.API.DTOs.Teams
{
    public class TeamDetailDTO : TeamDTO
    {
        public List<PlayerDTO> Players { get; set; } = new();
        public CoachDTO Coach { get; set; } = new();
    }
}
