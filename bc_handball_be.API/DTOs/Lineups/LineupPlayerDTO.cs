using bc_handball_be.API.DTOs.Players;

﻿namespace bc_handball_be.API.DTOs.Lineups
{
    public class LineupPlayerDTO
    {
        public int Id { get; set; }
        public int LineupId { get; set; }
        public PlayerDTO Player{ get; set; }

        // Additional properties can be added as needed
    }
}
