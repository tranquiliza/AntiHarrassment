using System;
using System.Security.Cryptography;
using System.Text;

namespace AntiHarassment.Core
{
    public class PasswordSecurity : ISecurity
    {
        public bool TryCreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            passwordHash = null;
            passwordSalt = null;

            if (password == null) return false;
            if (string.IsNullOrWhiteSpace(password)) return false;

            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }

            return true;
        }

        public bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            if (string.IsNullOrWhiteSpace(password)) return false;
            if (storedHash.Length != 64) throw new ArgumentException("Invalid length of password hash (64 bytes expected).", "passwordHash");
            if (storedSalt.Length != 128) throw new ArgumentException("Invalid length of password salt (128 bytes expected).", "passwordHash");

            using (var hmac = new HMACSHA512(storedSalt))
            {
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                    if (computedHash[i] != storedHash[i]) return false;
            }

            return true;
        }
    }
}
