using System;
using System.IO;
using System.Runtime.Serialization.Json;
using QueueIT.QueueToken.Model;

namespace QueueIT.QueueToken
{
    public class Token
    {
        public static EnqueueTokenGenerator Enqueue(String customerId) 
        {
            return new EnqueueTokenGenerator(customerId);
        }

        public static IEnqueueToken Parse(string token, string secretKey)
        {
            return EnqueueToken.Parse(token, secretKey);
        }
    }

    public class EnqueueTokenGenerator
    {
        private EnqueueToken _token;

        public EnqueueTokenGenerator(String customerId)
        {
            this._token = new EnqueueToken(customerId);
        }

        public EnqueueTokenGenerator WithEventId(String eventId)
        {
            this._token = new EnqueueToken(this._token, eventId);

            return this;
        }

        public EnqueueTokenGenerator WithValidity(long validityMillis)
        {
            this._token = new EnqueueToken(this._token, this._token.Issued.AddMilliseconds(validityMillis));

            return this;
        }

        public EnqueueTokenGenerator WithValidity(DateTime validity)
        {
            this._token = new EnqueueToken(this._token, validity);

            return this;
        }

        public EnqueueTokenGenerator WithPayload(IEnqueueTokenPayload payload)
        {
            this._token = new EnqueueToken(this._token, payload);

            return this;
        }

        public IEnqueueToken Generate(String secretKey)
        {
            _token.Generate(secretKey);
            return _token;
        }
    }

    public interface IEnqueueToken
    {
        TokenVersion TokenVersion { get; }
        EncryptionType Encryption { get; }
        DateTime Issued { get; }
        DateTime Expires { get; }
        string TokenIdentifier { get; }
        string CustomerId { get; }
        string EventId { get; }
        IEnqueueTokenPayload Payload { get; }
        string TokenWithoutHash { get; }
        string Token { get; }
        string HashCode { get; }
    }

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
        public string TokenWithoutHash { get; private set; }
        public string HashCode { get; private set; }
        public string Token => TokenWithoutHash + "." + HashCode;

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

            try
            {
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

                string serialized = dto.Serialize() + ".";
                if (Payload != null)
                {
                    serialized = serialized + Payload.EncryptAndEncode( secretKey, TokenIdentifier);
                }
                TokenWithoutHash = serialized;

                HashCode = Base64UrlEncoding.Encode(ShaHashing.GenerateHash(secretKey, TokenWithoutHash));
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
            var hashPart = tokenParts[2];

            if (string.IsNullOrEmpty(headerPart))
                throw new ArgumentException("Invalid token", nameof(tokenString));
            if (string.IsNullOrEmpty(hashPart))
                throw new ArgumentException("Invalid token", nameof(tokenString));

            var token = headerPart + "." + payloadPart;

            var expectedHash = Base64UrlEncoding.Encode(ShaHashing.GenerateHash(secretKey, token));

            if (expectedHash != hashPart)
                throw new InvalidHashException();

            try
            {
                var headerModel = HeaderDto.DeserializeHeader(headerPart);

                EnqueueTokenPayload payload = null;
                if (!string.IsNullOrEmpty(payloadPart))
                {
                    payload = EnqueueTokenPayload.Deserialize(payloadPart, secretKey, headerModel.TokenIdentifier);
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
                    TokenWithoutHash = token,
                    HashCode = expectedHash
                };
            }
            catch (Exception ex)
            {
                throw new TokenDeserializationException("Unable to deserialize token", ex);
            }
        }
    }

    public class TokenDeserializationException : Exception
    {
        public TokenDeserializationException(string message, Exception ex)
            : base(message, ex)
        {
        }
    }

    public class InvalidHashException : TokenDeserializationException
    {
        public InvalidHashException()
            : base("The token hash is invalid", null)
        {

        }
    }

    public class TokenSerializationException : Exception
    {
        public TokenSerializationException(Exception ex)
            : base("Exception serializing token", ex)
        {
        }
    }

}
