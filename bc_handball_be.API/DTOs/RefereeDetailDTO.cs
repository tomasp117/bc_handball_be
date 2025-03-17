namespace bc_handball_be.API.DTOs
{
    public class RefereeDetailDTO : RefereeDTO
    {
        public List<int> MainRefereeMatchIds { get; set; } = new();
        public List<int> AssistantRefereeMatchIds { get; set; } = new();
    }
}
