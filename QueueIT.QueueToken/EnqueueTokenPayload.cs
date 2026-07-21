using QueueIT.QueueToken.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace QueueIT.QueueToken
{
    public interface IEnqueueTokenPayload
    {
        string Key { get; }
        double? RelativeQuality { get; }
        IReadOnlyDictionary<string, string> CustomData { get; }
        TokenOrigin Origin { get; set; }
        string EncryptAndEncode(string secretKey, string tokenIdentifier);
    }

    internal class EnqueueTokenPayload : IEnqueueTokenPayload
    {
        private Dictionary<string, string> _customData;
        public string Key { get; private set; }
        public double? RelativeQuality { get; private set; }
        public TokenOrigin Origin { get; set; } = TokenOrigin.Connector;

        public EnqueueTokenPayload()
        {
            this._customData = new Dictionary<string, string>();
        }

        public EnqueueTokenPayload(EnqueueTokenPayload payload, string key)
        {
            this.Key = key;
            this.RelativeQuality = payload.RelativeQuality;
            this._customData = payload._customData;
        }

        public EnqueueTokenPayload(EnqueueTokenPayload payload, double relativeQuality)
        {
            this.Key = payload.Key;
            this.RelativeQuality = relativeQuality;
            this._customData = payload._customData;
        }

        public EnqueueTokenPayload(EnqueueTokenPayload payload, string customDataKey, string customDataValue)
        {
            this.Key = payload.Key;
            this.RelativeQuality = payload.RelativeQuality;
            this.Origin = payload.Origin;
            this._customData = payload._customData;
            this._customData.Add(customDataKey, customDataValue);
        }

        public EnqueueTokenPayload(string key, double? relativeQuality, Dictionary<string, string> customData)
        {
            this.Key = key;
            this.RelativeQuality = relativeQuality;
            this._customData = customData ?? new Dictionary<string, string>();
        }

        public EnqueueTokenPayload(EnqueueTokenPayload payload, TokenOrigin origin)
        {
            this.Origin = origin;
            this.Key = payload.Key;
            this.RelativeQuality = payload.RelativeQuality;
            this._customData = payload._customData;
        }

        public IReadOnlyDictionary<string, string> CustomData
        {
            get
            {
                return _customData.ToDictionary(arg => arg.Key, arg2 => arg2.Value);
            }
        }

        internal byte[] Serialize()
        {
            var dto = new PayloadDto()
            {
                Key = Key,
                RelativeQuality = RelativeQuality,
                Origin = Origin.ToString(),
                CustomData = _customData.Count > 0 ? _customData : null
            };

            return dto.Serialize();
        }

        public static EnqueueTokenPayload Deserialize(string input, string secretKey, string tokenIdentifier)
        {
            var dto = PayloadDto.DeserializePayload(input, secretKey, tokenIdentifier);
            var origin = TokenOrigin.Connector;
            if (!string.IsNullOrEmpty(dto.Origin))
            {
                if (!Enum.TryParse(dto.Origin, out origin))
                {
                    throw new ArgumentException("Invalid token origin");
                }
            }

            return new EnqueueTokenPayload
            {
                Key = dto.Key,
                RelativeQuality = dto.RelativeQuality,
                _customData = dto.CustomData ?? new Dictionary<string, string>(),
                Origin = origin
            };
        }

        public string EncryptAndEncode(string secretKey, string tokenIdentifier)
        {
            try
            {
                return EncryptStringPayload(Serialize(), secretKey, tokenIdentifier);
            }
            catch (Exception ex)
            {
                throw new TokenSerializationException(ex);
            }
        }

        public static string EncryptStringPayload(byte[] serializedPayloadBytes, string secretKey, string tokenIdentifier)
        {
            try
            {
                byte[] encrypted = AesEncryption.Encrypt(secretKey, tokenIdentifier, serializedPayloadBytes);
                return Base64UrlEncoding.Encode(encrypted);
            }
            catch (Exception ex)
            {
                throw new TokenSerializationException(ex);
            }
        }
    }
}