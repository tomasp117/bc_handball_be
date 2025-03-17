namespace bc_handball_be.API.DTOs
{
    public class PersonDetailDTO : PersonDTO
    {
        public string PasswordHash { get; set; } = string.Empty;
        public string Salt { get; set; } = string.Empty;
    }
}
