using System;

namespace QueueIT.QueueToken
{
    internal class TokenDeserializationException : Exception
    {
        public TokenDeserializationException(string message, Exception ex)
            : base(message, ex)
        {
        }
    }

    internal class InvalidSignatureException : TokenDeserializationException
    {
        public InvalidSignatureException()
            : base("The token signature is invalid", null)
        {

        }
    }
}