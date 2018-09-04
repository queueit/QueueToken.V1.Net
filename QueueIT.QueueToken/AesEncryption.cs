using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace QueueIT.QueueToken
{
    internal class AesEncryption
    {
        private readonly byte[] _key;
        private readonly byte[] _iv;

        public AesEncryption(string secretKey, string tokenIdentifier)
        {
            _key = GenerateKey(secretKey);
            _iv = GenerateIV(tokenIdentifier);
        }

        public byte[] DecryptPayload(byte[] encryptedBytes)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = _key;
                aesAlg.IV = _iv;

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

        public byte[] Encrypt(byte[] input)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = _key;
                aesAlg.IV = _iv;

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

        private byte[] GenerateKey(string secretKey)
        {
            var sha = SHA256.Create();
            return sha.ComputeHash(Encoding.UTF8.GetBytes(secretKey));
        }


        private byte[] GenerateIV(string tokenIdentifier)
        {
            var md5 = MD5.Create();
            return md5.ComputeHash(Encoding.UTF8.GetBytes(tokenIdentifier));
        }
    }
}
