namespace bc_handball_be.API.DTOs;

public class CategoryDetailDTO : CategoryDTO
{
    public List<GroupDTO> Groups { get; set; } = new();
    public List<PlayerDTO> Stats { get; set; } = new();
    public List<CoachDTO> Voting { get; set; } = new();
}