﻿namespace bc_handball_be.API.DTOs
{
    public class GroupSaveDTO
    {
        public string Name { get; set; } = string.Empty;
        public List<int> TeamIds { get; set; } = new();
    }
}
