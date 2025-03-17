using bc_handball_be.Core.Entities.IdentityField;
using System.Security.Cryptography;
using System.Text;

namespace bc_handball_be.Core.Entities.Actors.super
{
    public enum UserRole
    {
        Admin,
        Coach,
        Referee,
        Recorder
    }

    public class Person : BaseEntity
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public UserRole Role { get; set; }
        public string Salt { get; set; } = string.Empty;

        public void SetPassword(string password)
        {
            using (var rng = RandomNumberGenerator.Create())
            {
                byte[] saltBytes = new byte[16];
                rng.GetBytes(saltBytes);
                Salt = Convert.ToBase64String(saltBytes);
            }

            Password = Convert.ToBase64String(HashPassword(password, Convert.FromBase64String(Salt)));
        }

        public bool VerifyPassword(string password)
        {
            var hash = HashPassword(password, Convert.FromBase64String(Salt));
            return Convert.ToBase64String(hash) == Password;
        }

        private static byte[] HashPassword(string password, byte[] salt)
        {
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100000, HashAlgorithmName.SHA256))
            {
                return pbkdf2.GetBytes(32);
            }
        }
    }
}
