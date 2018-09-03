using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace QueueIT.QueueToken
{
    internal class EnqueueToken : IEnqueueToken
    {
        public string CustomerId { get; }
        public string EventId { get; }
        public DateTime Issued { get; private set; }
        public string TokenIdentifier { get; private set; }
        public string Token { get; private set; }
        public string Signature { get; private set; }
        public string SignedToken => Token + "." + Signature;
        public TokenVersion TokenVersion => TokenVersion.QT1;
        public EncryptionType Encryption => EncryptionType.AES256;
        public DateTime Expires { get; private set; }
        public IEnqueueTokenPayload Payload { get; }

        internal EnqueueToken(string customerId)
        {
            CustomerId = customerId;
            Issued = DateTime.UtcNow;
            Expires = DateTime.MaxValue;
            TokenIdentifier = Guid.NewGuid().ToString();
        }

        internal EnqueueToken(EnqueueToken token, DateTime expires)
            : this(token.CustomerId)
        {
            Issued = token.Issued;
            EventId = token.EventId;
            Payload = token.Payload;
            Expires = expires;
        }

        internal EnqueueToken(EnqueueToken token, string eventId)
            : this(token.CustomerId)
        {
            Issued = token.Issued;
            EventId = eventId;
            Payload = token.Payload;
            Expires = token.Expires;
        }

        internal EnqueueToken(EnqueueToken token, IEnqueueTokenPayload payload)
            : this(token.CustomerId)
        {
            Issued = token.Issued;
            EventId = token.EventId;
            Payload = payload;
            Expires = token.Expires;
        }

        internal EnqueueToken(string tokenIdentifier, string customerId, string eventId, DateTime issued, DateTime expires, IEnqueueTokenPayload payload)
        {
            TokenIdentifier = tokenIdentifier;
            CustomerId = customerId;
            EventId = eventId;
            Issued = issued;
            Expires = expires;
            Payload = payload;
        }

        internal void Generate(string secretKey, bool resetTokenIdentifier = true)
        {
            if (resetTokenIdentifier)
                TokenIdentifier = Guid.NewGuid().ToString();

            var md5 = MD5.Create();
            byte[] iv = md5.ComputeHash(Encoding.UTF8.GetBytes(TokenIdentifier));

            try
            {
                string serialized = SerializeHeader() + ".";
                if (Payload != null)
                {
                    string payloadJson = Payload.Serialize();
                    serialized = serialized + SerializePayload(payloadJson, secretKey, iv);
                }
                Token = serialized;

                var sha = SHA256.Create();
                var computedHash = sha.ComputeHash(Encoding.UTF8.GetBytes(Token + secretKey));
                Signature = Base64UrlEncoder.Encode(computedHash);
            }
            catch (Exception ex)
            {
                throw new TokenSerializationException(ex);
            }

        }

        private string SerializeHeader()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{");
            sb.Append("\"typ\":\"QT1\"");
            sb.Append(",\"enc\":\"AES256\"");
            sb.Append(",\"iss\":");
            sb.Append((new DateTimeOffset(Issued)).ToUnixTimeMilliseconds());
            if (Expires != DateTime.MaxValue)
            {
                sb.Append(",\"exp\":");
                sb.Append((new DateTimeOffset(Expires)).ToUnixTimeMilliseconds());
            }
            sb.Append(",\"ti\":");
            sb.Append(TokenIdentifier);
            sb.Append(",\"c\":");
            sb.Append(CustomerId);
            if (EventId!= null)
            {
                sb.Append(",\"e\":\"");
                sb.Append(EventId);
                sb.Append("\"");
            }
            sb.Append("}");

            return Base64UrlEncoder.Encode(Encoding.UTF8.GetBytes(sb.ToString()));
        }

        private string SerializePayload(string input, string secretKey, byte[] iv) 
        {   
            try {
                var sha = SHA256.Create();
                var key = sha.ComputeHash(Encoding.UTF8.GetBytes(Token + secretKey));
                byte[] encrypted = EncryptstringToBytes(input, key, iv);

                return Base64UrlEncoder.Encode(encrypted);

            } catch (Exception ex) {
                throw new TokenSerializationException(ex);
            }
        }

        private byte[] EncryptstringToBytes(string plainText, byte[] Key, byte[] iv)
        {
            byte[] encrypted;

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = iv;

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }
            return encrypted;

        }
    }
}

