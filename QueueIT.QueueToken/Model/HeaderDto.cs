using System;
using System.Runtime.Serialization;

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
    }
}
