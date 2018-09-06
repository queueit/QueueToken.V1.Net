using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using QueueIT.QueueToken.Model;
using System.Linq;

namespace QueueIT.QueueToken
{
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

        public IReadOnlyDictionary<string,string> CustomData
        {
            get
            {
                return this._customData.ToDictionary(arg => arg.Key, arg2 => arg2.Value);
            }
        }

        public byte[] Serialize()
        {
            var jsonSerializer = new DataContractJsonSerializer(typeof(PayloadDto), new DataContractJsonSerializerSettings()
            {
                UseSimpleDictionaryFormat = true
            });

            var dto = new PayloadDto()
            {
                Key = Key,
                Rank = Rank,
                CustomData = _customData.Count > 0 ? _customData : null
            };

            using (var stream = new MemoryStream())
            {
                jsonSerializer.WriteObject(stream, dto);
                return stream.ToArray();   
            }
        }

        public static EnqueueTokenPayload Deserialize(byte[] input)
        {
            var jsonSerializer = new DataContractJsonSerializer(typeof(Model.PayloadDto), new DataContractJsonSerializerSettings()
            {
                UseSimpleDictionaryFormat = true
            });

            using (var stream = new MemoryStream(input))
            {
                var dto = jsonSerializer.ReadObject(stream) as Model.PayloadDto;

                return new EnqueueTokenPayload(dto.Key, dto.Rank, dto.CustomData);
            }
        }
    }
}

