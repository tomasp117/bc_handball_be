namespace bc_handball_be.API.DTOs.Persons
{
    /// <summary>
    /// Detailed person DTO for API responses.
    /// WARNING: Never include authentication secrets (PasswordHash, Salt) in DTOs exposed via API.
    /// </summary>
    public class PersonDetailDTO : PersonDTO
    {
        // PasswordHash and Salt removed for security - these should NEVER be exposed via API
    }
}
