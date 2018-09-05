using System.Security.Cryptography;
using System.Text;

namespace QueueIT.QueueToken
{
    internal class ShaSignature
    {
        private readonly string _secretKey;

        public ShaSignature(string secretKey)
        {
            _secretKey = secretKey;
        }

        public byte[] GenerateSignature(string tokenString)
        {
            var sha = SHA256.Create();
            return sha.ComputeHash(Encoding.UTF8.GetBytes(tokenString + _secretKey));
        }

    }
}
