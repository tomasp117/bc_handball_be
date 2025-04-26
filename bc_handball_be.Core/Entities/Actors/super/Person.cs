using bc_handball_be.Core.Entities.IdentityField;
using System.Security.Cryptography;
using System.Text;

namespace bc_handball_be.Core.Entities.Actors.super
{

    public class Person : BaseEntity
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; } = string.Empty;
        public string? Address { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }

        public Login? Login { get; set; }

    }
}
