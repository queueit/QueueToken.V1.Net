using System;
using System.IO;
using System.Runtime.Serialization.Json;
using QueueIT.QueueToken.Model;

namespace QueueIT.QueueToken
{
    internal class EnqueueToken : IEnqueueToken
    {
        public string CustomerId { get; }
        public string EventId { get; }
        public DateTime Issued { get; }
        public string TokenIdentifier { get; private set; }
        public TokenVersion TokenVersion => TokenVersion.QT1;
        public EncryptionType Encryption => EncryptionType.AES256;
        public DateTime Expires { get; }
        public IEnqueueTokenPayload Payload { get; }
        public string Token { get; private set; }
        public string Signature { get; private set; }
        public string SignedToken => Token + "." + Signature;

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

        internal EnqueueToken(string tokenIdentifier, string customerId, string eventId, DateTime issued, DateTime? expires, IEnqueueTokenPayload payload)
        {
            TokenIdentifier = tokenIdentifier;
            CustomerId = customerId;
            EventId = eventId;
            Issued = issued;
            Expires = expires ?? DateTime.MaxValue;
            Payload = payload;
        }

        internal void Generate(string secretKey, bool resetTokenIdentifier = true)
        {
            if (resetTokenIdentifier)
                TokenIdentifier = Guid.NewGuid().ToString();

            var aes = new AesEncryption(secretKey, TokenIdentifier);
            var sha = new ShaSignature(secretKey);
            
            try
            {
                string serialized = SerializeHeader() + ".";
                if (Payload != null)
                {
                    byte[] serializedJayload = Payload.Serialize();
                    serialized = serialized + EncryptAndEncode(serializedJayload, aes);
                }
                Token = serialized;

                Signature = sha.GenerateSignature(Token);
            }
            catch (Exception ex)
            {
                throw new TokenSerializationException(ex);
            }
        }

        public static IEnqueueToken Parse(string tokenString, string secretKey)
        {
            if (string.IsNullOrEmpty(secretKey))
                throw new ArgumentException("Invalid secret key", nameof(secretKey));

            if (string.IsNullOrEmpty(tokenString))
                throw new ArgumentException("Invalid token", nameof(tokenString));

            var tokenParts = tokenString.Split('.');
            var headerPart = tokenParts[0];
            var payloadPart = tokenParts[1];
            var signaturePart = tokenParts[2];

            if (string.IsNullOrEmpty(headerPart))
                throw new ArgumentException("Invalid token", nameof(tokenString));
            if (string.IsNullOrEmpty(signaturePart))
                throw new ArgumentException("Invalid token", nameof(tokenString));

            var token = headerPart + "." + payloadPart;

            var sha = new ShaSignature(secretKey);
            var signature = sha.GenerateSignature(token);

            if (signature != signaturePart)
                throw new InvalidSignatureException();

            try
            {
                var headerModel = DeserializeHeader(headerPart);

                var aes = new AesEncryption(secretKey, headerModel.TokenIdentifier);

                EnqueueTokenPayload payload = null;
                if (!string.IsNullOrEmpty(payloadPart))
                {
                    payload = DeserializePayload(payloadPart, aes);
                }

                return new EnqueueToken(
                    headerModel.TokenIdentifier,
                    headerModel.CustomerId,
                    headerModel.EventId,
                    DateTimeOffset.FromUnixTimeMilliseconds(headerModel.Issued).DateTime,
                    headerModel.Expires.HasValue
                        ? new DateTime?(DateTimeOffset.FromUnixTimeMilliseconds(headerModel.Expires.Value).DateTime)
                        : null,
                    payload)
                {
                    Token = token,
                    Signature = signature
                };
            }
            catch (Exception ex)
            {
                throw new TokenDeserializationException("Unable to deserialize token", ex);
            }
        }

        private static EnqueueTokenPayload DeserializePayload(string input, AesEncryption aes)
        {
            var headerEncrypted = Base64UrlEncoding.Decode(input);
            var decrypted = aes.DecryptPayload(headerEncrypted);
            return EnqueueTokenPayload.Deserialize(decrypted);

        }

        private static HeaderDto DeserializeHeader(string input)
        {
            var jsonSerializer = new DataContractJsonSerializer(typeof(HeaderDto));

            var headerJson = Base64UrlEncoding.Decode(input);

            using (var stream = new MemoryStream(headerJson))
            {
                return jsonSerializer.ReadObject(stream) as HeaderDto;
            }
        }

        private string SerializeHeader()
        {
            var jsonSerializer = new DataContractJsonSerializer(typeof(HeaderDto));
            var dto = new HeaderDto()
            {
                CustomerId = CustomerId,
                EventId = EventId,
                TokenIdentifier = TokenIdentifier,
                Issued = (new DateTimeOffset(Issued)).ToUnixTimeMilliseconds(),
                Expires = Expires == DateTime.MaxValue ? null : (long?)(new DateTimeOffset(Expires)).ToUnixTimeMilliseconds(),
                Encryption = EncryptionType.AES256.ToString(),
                TokenVersion = TokenVersion.QT1.ToString()
            };

            using (var stream = new MemoryStream())
            {
                jsonSerializer.WriteObject(stream, dto);

                return Base64UrlEncoding.Encode(stream.ToArray());
            }
        }

        private string EncryptAndEncode(byte[] input, AesEncryption aes)
        {
            try
            {
                byte[] encrypted = aes.Encrypt(input);

                return Base64UrlEncoding.Encode(encrypted);

            }
            catch (Exception ex)
            {
                throw new TokenSerializationException(ex);
            }
        }
    }
}

