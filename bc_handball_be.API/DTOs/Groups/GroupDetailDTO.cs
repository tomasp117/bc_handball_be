using bc_handball_be.API.DTOs.Teams;
using bc_handball_be.API.DTOs.Matches;

﻿namespace bc_handball_be.API.DTOs.Groups
{
    public class GroupDetailDTO : GroupDTO
    {
        public List<TeamDTO> Teams { get; set; } = new();
        public List<MatchDTO> Matches { get; set; } = new();
    }
}
