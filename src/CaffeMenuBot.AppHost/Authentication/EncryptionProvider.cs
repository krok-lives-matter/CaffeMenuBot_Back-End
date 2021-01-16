using System;
using System.Security.Cryptography;
using System.Text;

namespace CaffeMenuBot.AppHost.Authentication
{
    internal static class EncryptionProvider
    {
        public static byte[] ReadSaltFromBase64(string saltBase64)
        {
            return Convert.FromBase64String(saltBase64);
        }

        public static string GenerateSaltForUser(string email)
        {
            var emailBytes = Encoding.ASCII.GetBytes(email);
            return Convert.ToBase64String(emailBytes);
        }

        public static string Encrypt(string passwordString, byte[] salt)
        {
            HashAlgorithm algorithm = new SHA256Managed();

            byte[] password = Encoding.ASCII.GetBytes(passwordString);

            byte[] passwordWithSaltBytes = new byte[password.Length + salt.Length];

            for (int i = 0; i < password.Length; i++)
            {
                passwordWithSaltBytes[i] = password[i];
            }
            for (int i = 0; i < salt.Length; i++)
            {
                passwordWithSaltBytes[password.Length + i] = salt[i];
            }

            return Convert.ToBase64String(algorithm.ComputeHash(passwordWithSaltBytes));
        }
    }
}
