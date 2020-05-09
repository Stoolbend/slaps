using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.Security.Cryptography;

namespace BCI.SLAPS.Server.Helpers
{
    public static class PasswordUtils
    {
        public static byte[] GenerateSalt(int length = 32)
        {
            byte[] salt = new byte[length];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            return salt;
        }

        public static string GenerateHash(string password, byte[] salt)
        {
            return  Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));
        }
    }
}
