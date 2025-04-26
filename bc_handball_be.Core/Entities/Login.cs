using bc_handball_be.Core.Entities.Actors.super;
using bc_handball_be.Core.Entities.IdentityField;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace bc_handball_be.Core.Entities
{
    public class Login : BaseEntity
    {

        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Salt { get; set; } = string.Empty;

        // Foreign keys
        public int PersonId { get; set; }
        public Person Person { get; set; } = null!;

        public void SetPassword(string password)
        {
            using (var rng = RandomNumberGenerator.Create())
            {
                byte[] saltBytes = new byte[16];
                rng.GetBytes(saltBytes);
                Salt = Convert.ToBase64String(saltBytes);
            }

            var hashed = HashPassword(password, Convert.FromBase64String(Salt));
            Password = Convert.ToBase64String(hashed);

            Console.WriteLine($"[REGISTER] Password hash: {Password}");
            Console.WriteLine($"[REGISTER] Salt: {Salt}");
        }

        public bool VerifyPassword(string password)
        {
            var saltBytes = Convert.FromBase64String(Salt);
            var hash = HashPassword(password, saltBytes);
            var hashString = Convert.ToBase64String(hash);

            Console.WriteLine($"[LOGIN] Comparing hash: {hashString}");
            Console.WriteLine($"[LOGIN] Stored hash: {Password}");

            return hashString == Password;
        }

        private static byte[] HashPassword(string password, byte[] salt)
        {
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100_000, HashAlgorithmName.SHA256))
            {
                return pbkdf2.GetBytes(32);
            }
        }
    }
}
