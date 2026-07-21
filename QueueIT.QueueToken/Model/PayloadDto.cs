using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace QueueIT.QueueToken.Model
{
    [DataContract]
    public class PayloadDto
    {
        [DataMember(Name = "r", Order = 1, EmitDefaultValue = false)]
        public double? RelativeQuality { get; set; }
        [DataMember(Name = "k", Order = 2, EmitDefaultValue = false)]
        public string Key { get; set; }
        [DataMember(Name = "cd", Order = 3, EmitDefaultValue = false)]
        public Dictionary<string, string> CustomData { get; set; }
        [DataMember(Name = "O", Order = 4, EmitDefaultValue = false)]
        public string Origin { get; set; }

        internal static PayloadDto DeserializePayload(string input, string secretKey, string tokenIdentifier)
        {
            var headerEncrypted = Base64UrlEncoding.Decode(input);
            var decrypted = AesEncryption.DecryptPayload(secretKey, tokenIdentifier, headerEncrypted);

            var jsonSerializer = new DataContractJsonSerializer(typeof(PayloadDto), new DataContractJsonSerializerSettings()
            {
                UseSimpleDictionaryFormat = true
            });

            using (var stream = new MemoryStream(decrypted))
            {
                return jsonSerializer.ReadObject(stream) as PayloadDto;
            }
        }

        internal byte[] Serialize()
        {
            var jsonSerializer = new DataContractJsonSerializer(typeof(PayloadDto), new DataContractJsonSerializerSettings()
            {
                UseSimpleDictionaryFormat = true
            });

            using (var stream = new MemoryStream())
            {
                jsonSerializer.WriteObject(stream, this);
                return stream.ToArray();
            }
        }
    }
}
