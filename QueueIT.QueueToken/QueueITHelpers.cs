using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace QueueIT.QueueToken
{
    internal static class AesEncryption
    {
        public static byte[] DecryptPayload(string secretKey, string tokenIdentifier, byte[] encryptedBytes)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = GenerateKey(secretKey);
                aesAlg.IV = GenerateIV(tokenIdentifier);

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, decryptor, CryptoStreamMode.Write))
                    {
                        csEncrypt.Write(encryptedBytes, 0, encryptedBytes.Length);
                    }

                    return msEncrypt.ToArray();
                }
            }
        }

        public static byte[] Encrypt(string secretKey, string tokenIdentifier, byte[] input)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = GenerateKey(secretKey);
                aesAlg.IV = GenerateIV(tokenIdentifier);

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        csEncrypt.Write(input, 0, input.Length);
                    }
                    return msEncrypt.ToArray();
                }
            }
        }

        private static byte[] GenerateKey(string secretKey)
        {
            var sha = SHA256.Create();
            return sha.ComputeHash(Encoding.UTF8.GetBytes(secretKey));
        }


        private static byte[] GenerateIV(string tokenIdentifier)
        {
            var md5 = MD5.Create();
            return md5.ComputeHash(Encoding.UTF8.GetBytes(tokenIdentifier));
        }
    }

    internal static class Base64UrlEncoding
    {
        public static String Encode(byte[] input)
        {
            return Convert.ToBase64String(input)
                .Replace('+', '-')
                .Replace('/', '_')
                .TrimEnd('=');
        }

        public static byte[] Decode(string input)
        {
            var base64 = input
                .Replace('-', '+')
                .Replace('_', '/');

            var padding = base64.Length % 4;
            if (padding == 3)
                padding = 1;

            base64 = base64.PadRight(base64.Length + padding, '=');

            return Convert.FromBase64String(base64);
        }
    }

    internal static class ShaHashing
    {
        public static byte[] GenerateHash(string secretKey, string tokenString)
        {
            var sha = SHA256.Create();
            return sha.ComputeHash(Encoding.UTF8.GetBytes(tokenString + secretKey));
        }
    }
}
