using System;

namespace QueueIT.QueueToken
{
    public interface IEnqueueTokenPayload
    {
        string Key { get; }
        double? Rank { get; }
        string GetCustomDataValue(String key);
        byte[] Serialize();
    }
}
