namespace bc_handball_be.API.DTOs
{
    public class CoachDTO
    {
        public char License { get; set; }
        public int personId { get; set; }
        public PersonDTO Person { get; set; } = new();
    }
}
