using bc_handball_be.API.DTOs.Persons;

namespace bc_handball_be.API.DTOs.Clubs
{
    public class ClubAdminDTO
    {
        public int Id { get; set; }
        public PersonDTO Person { get; set; }
        public int ClubId { get; set; }
    }
}
