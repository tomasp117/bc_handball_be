using bc_handball_be.API.DTOs.Groups;
using bc_handball_be.API.DTOs.Players;
using bc_handball_be.API.DTOs.Coaches;

namespace bc_handball_be.API.DTOs.Categories;

public class CategoryDetailDTO : CategoryDTO
{
    public List<GroupDTO> Groups { get; set; } = new();
    public List<PlayerDTO> Stats { get; set; } = new();
    public List<CoachDTO> Voting { get; set; } = new();
}