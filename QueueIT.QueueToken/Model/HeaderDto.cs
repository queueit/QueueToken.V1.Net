using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace QueueIT.QueueToken.Model
{
    [DataContract]
    public class HeaderDto
    {
        [DataMember(Name = "typ", Order = 1)]
        public string TokenVersion { get; set; }
        [DataMember(Name = "enc", Order = 2)]
        public string Encryption { get; set; }
        [DataMember(Name = "iss", Order = 3)]
        public long Issued { get; set; }
        [DataMember(Name = "exp", Order = 4, EmitDefaultValue = false)]
        public long? Expires { get; set; }
        [DataMember(Name = "ti", Order = 5)]
        public string TokenIdentifier { get; set; }
        [DataMember(Name= "c", Order = 6)]
        public string CustomerId { get; set; }
        [DataMember(Name = "e", Order = 7, EmitDefaultValue = false)]
        public string EventId { get; set; }
        [DataMember(Name = "ip", Order = 8, EmitDefaultValue = false)]
        public string IpAddress { get; set; }

        internal static HeaderDto DeserializeHeader(string input)
        {
            var jsonSerializer = new DataContractJsonSerializer(typeof(HeaderDto));

            var headerJson = Base64UrlEncoding.Decode(input);

            using (var stream = new MemoryStream(headerJson))
            {
                return jsonSerializer.ReadObject(stream) as HeaderDto;
            }
        }

        internal string Serialize()
        {
            var jsonSerializer = new DataContractJsonSerializer(typeof(HeaderDto));

            using (var stream = new MemoryStream())
            {
                jsonSerializer.WriteObject(stream, this);

                return Base64UrlEncoding.Encode(stream.ToArray());
            }
        }

    }
}
