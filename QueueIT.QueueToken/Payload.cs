using System;
using System.Collections.Generic;
using System.Text;

namespace QueueIT.QueueToken
{
    public class Payload
    {
        public static EnqueueTokenPayloadGenerator Enqueue() => new EnqueueTokenPayloadGenerator();
    }
}