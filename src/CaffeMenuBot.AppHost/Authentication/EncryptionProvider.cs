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

        public static string Encrypt(string _passwordString, byte[] salt)
        {
            HashAlgorithm algorithm = new SHA256Managed();

            byte[] _password = Encoding.ASCII.GetBytes(_passwordString);

            byte[] passwordWithSaltBytes = new byte[_password.Length + salt.Length];

            for (int i = 0; i < _password.Length; i++)
            {
                passwordWithSaltBytes[i] = _password[i];
            }
            for (int i = 0; i < salt.Length; i++)
            {
                passwordWithSaltBytes[_password.Length + i] = salt[i];
            }

            return Convert.ToBase64String(algorithm.ComputeHash(passwordWithSaltBytes));
        }
    }
}
