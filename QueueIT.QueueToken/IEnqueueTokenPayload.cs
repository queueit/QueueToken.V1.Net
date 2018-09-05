using System;
using System.Collections.Generic;

namespace QueueIT.QueueToken
{
    public interface IEnqueueTokenPayload
    {
        string Key { get; }
        double? Rank { get; }
        Dictionary<string, string> GetCustomDataDictionary();
        byte[] Serialize();
    }
}
