using System;
using System.Collections.Generic;

namespace QueueIT.QueueToken
{
    public interface IEnqueueTokenPayload
    {
        string Key { get; }
        double? Rank { get; }
        IReadOnlyDictionary<string, string> CustomData { get; }
        byte[] Serialize();
    }
}
