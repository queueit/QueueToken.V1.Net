using System;

namespace QueueIT.QueueToken
{
    internal static class Base64UrlEncoder
    {
        public static String Encode(byte[] input)
        {
            return Convert.ToBase64String(input)
                .Replace('+', '-')
                .Replace('/', '_')
                .TrimEnd('=');
        }
    }
}
