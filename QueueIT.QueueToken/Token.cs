using System;

namespace QueueIT.QueueToken
{
    public class Token
    {
        public static EnqueueTokenGenerator Enqueue(String customerId) 
        {
            return new EnqueueTokenGenerator(customerId);
        }
        public static IEnqueueToken Parse(string token, string secretKey)
        {
            return EnqueueToken.Parse(token, secretKey);
        }
    }
}
