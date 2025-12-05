using bc_handball_be.API.DTOs.Persons;

namespace bc_handball_be.API.DTOs.Coaches
{
    public class CoachDTO
    {
        public int Id { get; set; }
        public char License { get; set; }
        public PersonDTO Person { get; set; } = new();
    }
}
