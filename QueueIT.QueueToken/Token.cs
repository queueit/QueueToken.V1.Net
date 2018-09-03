using System;

namespace QueueIT.QueueToken
{
    public class Token
    {
        public static EnqueueTokenGenerator Enqueue(String customerId) 
        {
            return new EnqueueTokenGenerator(customerId);
        }
    }
}
