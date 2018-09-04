using System.Collections.Generic;
using System.Runtime.Serialization;

namespace QueueIT.QueueToken.Model
{
    [DataContract]
    public class PayloadDto
    {
        [DataMember(Name = "r", Order = 1, EmitDefaultValue = false)]
        public double? Rank { get; set; }
        [DataMember(Name = "k", Order = 2, EmitDefaultValue = false)]
        public string Key { get; set; }
        [DataMember(Name = "cd", Order = 3, EmitDefaultValue = false)]
        public Dictionary<string, string> CustomData { get; set; }
    }
}
