using System;
using System.Security.Cryptography;

namespace Common
{
    public static class PasswordHasher
    {
        private const int SaltSize = 16; // 128 bit
        private const int KeySize = 32; // 256 bit
        private const int Iterations = 10000;

        public static void HashPassword(string password, out string hash, out string salt)
        {
            using (var algorithm = new Rfc2898DeriveBytes(password, SaltSize, Iterations, HashAlgorithmName.SHA256))
            {
                var key = Convert.ToBase64String(algorithm.GetBytes(KeySize));
                var saltBytes = Convert.ToBase64String(algorithm.Salt);

                hash = key;
                salt = saltBytes;
            }
        }

        public static bool VerifyPassword(string password, string hash, string salt)
        {
            var saltBytes = Convert.FromBase64String(salt);

            using (var algorithm = new Rfc2898DeriveBytes(password, saltBytes, Iterations, HashAlgorithmName.SHA256))
            {
                var keyToCheck = algorithm.GetBytes(KeySize);
                var key = Convert.FromBase64String(hash);

                if (keyToCheck.Length != key.Length)
                    return false;

                for (int i = 0; i < keyToCheck.Length; i++)
                {
                    if (keyToCheck[i] != key[i])
                        return false;
                }
                return true;
            }
        }
    }
}
