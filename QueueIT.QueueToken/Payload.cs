using System;
using System.Collections.Generic;
using System.Linq;
using QueueIT.QueueToken.Model;

namespace QueueIT.QueueToken
{
    public class Payload
    {
        public static EnqueueTokenPayloadGenerator Enqueue() => new EnqueueTokenPayloadGenerator();
    }

    public class EnqueueTokenPayloadGenerator
    {
        private EnqueueTokenPayload _payload;

        public EnqueueTokenPayloadGenerator()
        {
            this._payload = new EnqueueTokenPayload();
        }

        public EnqueueTokenPayloadGenerator WithKey(String key)
        {
            this._payload = new EnqueueTokenPayload(this._payload, key);

            return this;
        }

        public EnqueueTokenPayloadGenerator WithRelativeQuality(double relativeQuality)
        {
            this._payload = new EnqueueTokenPayload(this._payload, relativeQuality);

            return this;
        }

        public EnqueueTokenPayloadGenerator WithCustomData(String key, String value)
        {
            this._payload = new EnqueueTokenPayload(this._payload, key, value);

            return this;
        }

        public IEnqueueTokenPayload Generate()
        {
            return this._payload;
        }
    }

    public interface IEnqueueTokenPayload
    {
        string Key { get; }
        double? RelativeQuality { get; }
        IReadOnlyDictionary<string, string> CustomData { get; }
        string EncryptAndEncode(string secretKey, string tokenIdentifier);
    }

    internal class EnqueueTokenPayload : IEnqueueTokenPayload
    {
        public string Key { get; }
        public double? RelativeQuality { get; }
        private readonly Dictionary<string, string> _customData;

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
            this._customData = payload._customData;
            this._customData.Add(customDataKey, customDataValue);
        }

        public EnqueueTokenPayload(string key, double? relativeQuality, Dictionary<string, string> customData)
        {
            this.Key = key;
            this.RelativeQuality = relativeQuality;
            this._customData = customData ?? new Dictionary<string, string>();
        }

        public IReadOnlyDictionary<string, string> CustomData
        {
            get
            {
                return this._customData.ToDictionary(arg => arg.Key, arg2 => arg2.Value);
            }
        }

        internal byte[] Serialize()
        {
            var dto = new PayloadDto()
            {
                Key = Key,
                RelativeQuality = RelativeQuality,
                CustomData = _customData.Count > 0 ? _customData : null
            };

            return dto.Serialize();
        }

        public static EnqueueTokenPayload Deserialize(string input, string secretKey, string tokenIdentifier)
        {
            var dto = PayloadDto.DeserializePayload(input, secretKey, tokenIdentifier);
            return new EnqueueTokenPayload(dto.Key, dto.RelativeQuality, dto.CustomData);
        }

        public string EncryptAndEncode(string secretKey, string tokenIdentifier)
        {
            try
            {
                byte[] encrypted = AesEncryption.Encrypt(secretKey, tokenIdentifier, Serialize());

                return Base64UrlEncoding.Encode(encrypted);

            }
            catch (Exception ex)
            {
                throw new TokenSerializationException(ex);
            }
        }
    }
}