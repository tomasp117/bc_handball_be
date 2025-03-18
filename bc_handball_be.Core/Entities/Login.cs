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
