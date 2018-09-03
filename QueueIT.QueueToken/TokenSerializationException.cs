using System;

namespace QueueIT.QueueToken
{
    public class TokenSerializationException : Exception
    {
        public TokenSerializationException(Exception ex)
            :base("Exception serializing token", ex)
        {
        }
    }
}
