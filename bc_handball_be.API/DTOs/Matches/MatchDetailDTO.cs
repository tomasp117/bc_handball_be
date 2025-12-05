using bc_handball_be.API.DTOs.Events;

﻿namespace bc_handball_be.API.DTOs.Matches
{
    public class MatchDetailDTO : MatchDTO
    {
        public List<EventDTO> Events { get; set; } = new();
        public int? MainRefereeId { get; set; }
        public string? MainRefereeName { get; set; }
        public int? AssistantRefereeId { get; set; }
        public string? AssistantRefereeName { get; set; }
    }
}
