using System.Security.Cryptography;

namespace Server.Utill
{
    public class PasswordHasher
    {
        private readonly int iterations;
        private readonly int hashSize;

        public PasswordHasher(int iterations, int hashSize)
        {
            this.iterations = iterations;
            this.hashSize = hashSize;
        }

        public byte[] HashPassword(string password, byte[] salt)
        {
            using var pbkdf2 = new Rfc2898DeriveBytes(
                password,
                salt,
                iterations,
                HashAlgorithmName.SHA256);

            return pbkdf2.GetBytes(hashSize);
        }

        public bool VerifyPassword(string password, byte[] salt, byte[] expectedHash)
        {
            var computedHash = HashPassword(password, salt);
            return CryptographicOperations.FixedTimeEquals(computedHash, expectedHash);
        }
    }
}
