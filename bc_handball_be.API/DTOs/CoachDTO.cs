namespace bc_handball_be.API.DTOs
{
    public class CoachDTO
    {
        public int Id { get; set; }
        public char License { get; set; }
        public PersonDTO Person { get; set; } = new();
    }
}
