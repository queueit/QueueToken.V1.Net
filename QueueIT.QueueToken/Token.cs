using System;
using System.IO;
using System.Runtime.Serialization.Json;
using QueueIT.QueueToken.Model;

namespace QueueIT.QueueToken
{
    public class Token
    {
        public static EnqueueTokenGenerator Enqueue(string customerId, string tokenIdentifierPrefix = null) 
        {
            return new EnqueueTokenGenerator(customerId, tokenIdentifierPrefix);
        }

        public static IEnqueueToken Parse(string token, string secretKey)
        {
            return EnqueueToken.Parse(token, secretKey);
        }
    }

    public class EnqueueTokenGenerator
    {
        private EnqueueToken _token;

        public EnqueueTokenGenerator(string customerId, string tokenIdentifierPrefix = null)
        {
            this._token = new EnqueueToken(customerId, tokenIdentifierPrefix);
        }

        public EnqueueTokenGenerator WithEventId(string eventId)
        {
            this._token = EnqueueToken.AddEventId(this._token, eventId);

            return this;
        }

        public EnqueueTokenGenerator WithValidity(long validityMillis)
        {
            this._token = EnqueueToken.AddExpires(this._token, this._token.Issued.AddMilliseconds(validityMillis));

            return this;
        }

        public EnqueueTokenGenerator WithValidity(DateTime validity)
        {
            this._token = EnqueueToken.AddExpires(this._token, validity);

            return this;
        }

        public EnqueueTokenGenerator WithPayload(IEnqueueTokenPayload payload)
        {
            this._token = EnqueueToken.AddPayload(this._token, payload);

            return this;
        }

        public IEnqueueToken Generate(string secretKey)
        {
            _token.Generate(secretKey);
            return _token;
        }

        public EnqueueTokenGenerator WithIpAddress(string ipAddress)
        {
            this._token = EnqueueToken.AddIPAddress(this._token, ipAddress);

            return this;
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
        string IpAddress { get; }
        IEnqueueTokenPayload Payload { get; }
        string TokenWithoutHash { get; }
        string Token { get; }
        string HashCode { get; }
    }

    internal class EnqueueToken : IEnqueueToken
    {
        private readonly string _tokenIdentifierPrefix;
        public string CustomerId { get; }
        public string EventId { get; }
        public string IpAddress { get; }
        public DateTime Issued { get; }
        public string TokenIdentifier { get; private set; }
        public TokenVersion TokenVersion => TokenVersion.QT1;
        public EncryptionType Encryption => EncryptionType.AES256;
        public DateTime Expires { get; }
        public IEnqueueTokenPayload Payload { get; }
        public string TokenWithoutHash { get; private set; }
        public string HashCode { get; private set; }
        public string Token => TokenWithoutHash + "." + HashCode;

        internal EnqueueToken(string customerId, string tokenIdentifierPrefix)
        {
            _tokenIdentifierPrefix = tokenIdentifierPrefix;
            CustomerId = customerId;
            Issued = DateTime.UtcNow;
            Expires = DateTime.MaxValue;
            TokenIdentifier = GetTokenIdentifier(tokenIdentifierPrefix);
        }

        private static string GetTokenIdentifier(string tokenIdentifierPrefix)
        {
            return string.IsNullOrEmpty(tokenIdentifierPrefix) 
                ? Guid.NewGuid().ToString()
                : $"{tokenIdentifierPrefix}~{Guid.NewGuid()}";
        }

        internal EnqueueToken(string tokenIdentifier, string customerId, string eventId, DateTime issued, DateTime? expires, string ipAddress, IEnqueueTokenPayload payload)
        {
            TokenIdentifier = tokenIdentifier;
            CustomerId = customerId;
            EventId = eventId;
            Issued = issued;
            Expires = expires ?? DateTime.MaxValue;
            Payload = payload;
            IpAddress = ipAddress;
        }

        internal void Generate(string secretKey, bool resetTokenIdentifier = true)
        {
            if (resetTokenIdentifier)
                TokenIdentifier = GetTokenIdentifier(_tokenIdentifierPrefix);

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
                    TokenVersion = TokenVersion.QT1.ToString(),
                    IpAddress = IpAddress
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
                    headerModel.IpAddress,
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

        internal static EnqueueToken AddIPAddress(EnqueueToken token, string ipAddress)
        {
            return new EnqueueToken(token.TokenIdentifier, token.CustomerId, token.EventId, token.Issued, token.Expires, ipAddress, token.Payload);
        }
        internal static EnqueueToken AddEventId(EnqueueToken token, string eventId)
        {
            return new EnqueueToken(token.TokenIdentifier, token.CustomerId, eventId, token.Issued, token.Expires, token.IpAddress, token.Payload);
        }
        internal static EnqueueToken AddExpires(EnqueueToken token, DateTime expires)
        {
            return new EnqueueToken(token.TokenIdentifier, token.CustomerId, token.EventId, token.Issued, expires, token.IpAddress, token.Payload);
        }
        internal static EnqueueToken AddPayload(EnqueueToken token, IEnqueueTokenPayload payload)
        {
            return new EnqueueToken(token.TokenIdentifier, token.CustomerId, token.EventId, token.Issued, token.Expires, token.IpAddress, payload);
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
