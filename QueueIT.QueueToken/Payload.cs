using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
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

        public EnqueueTokenPayloadGenerator WithRank(double rank)
        {
            this._payload = new EnqueueTokenPayload(this._payload, rank);

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
        double? Rank { get; }
        string GetCustomDataValue(String key);
        string EncryptAndEncode(string secretKey, string tokenIdentifier);
    }

    internal class EnqueueTokenPayload : IEnqueueTokenPayload
    {
        public string Key { get; }
        public double? Rank { get; }
        private Dictionary<string, string> _customData;

        public EnqueueTokenPayload()
        {
            this._customData = new Dictionary<string, string>();
        }

        public EnqueueTokenPayload(EnqueueTokenPayload payload, string key)
        {
            this.Key = key;
            this.Rank = payload.Rank;
            this._customData = payload._customData;
        }

        public EnqueueTokenPayload(EnqueueTokenPayload payload, double rank)
        {
            this.Key = payload.Key;
            this.Rank = rank;
            this._customData = payload._customData;
        }

        public EnqueueTokenPayload(EnqueueTokenPayload payload, string customDataKey, string customDataValue)
        {
            this.Key = payload.Key;
            this.Rank = payload.Rank;
            this._customData = payload._customData;
            this._customData.Add(customDataKey, customDataValue);
        }

        public EnqueueTokenPayload(string key, double? rank, Dictionary<string, string> customData)
        {
            this.Key = key;
            this.Rank = rank;
            this._customData = customData;
        }

        public string GetCustomDataValue(string key)
        {
            if (!_customData.ContainsKey(key))
                return null;
            return this._customData[key];
        }

        internal byte[] Serialize()
        {
            var dto = new PayloadDto()
            {
                Key = Key,
                Rank = Rank,
                CustomData = _customData.Count > 0 ? _customData : null
            };

            return dto.Serialize();
        }

        public static EnqueueTokenPayload Deserialize(string input, string secretKey, string tokenIdentifier)
        {
            var dto = PayloadDto.DeserializePayload(input, secretKey, tokenIdentifier);
            return new EnqueueTokenPayload(dto.Key, dto.Rank, dto.CustomData);
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