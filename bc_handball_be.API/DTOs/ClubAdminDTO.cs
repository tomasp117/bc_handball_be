namespace bc_handball_be.API.DTOs
{
    public class ClubAdminDTO
    {
        public int Id { get; set; }
        public PersonDTO Person { get; set; }
        public int ClubId { get; set; }
    }
}
