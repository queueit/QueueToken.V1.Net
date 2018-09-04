using System;

namespace QueueIT.QueueToken
{
    internal static class Base64UrlEncoding
    {
        public static String Encode(byte[] input)
        {
            return Convert.ToBase64String(input)
                .Replace('+', '-')
                .Replace('/', '_')
                .TrimEnd('=');
        }

        public static byte[] Decode(string input)
        {
            var base64 = input
                .Replace('-', '+')
                .Replace('_', '/');

            var padding = base64.Length % 4;
            if (padding == 3)
                padding = 1;

            base64 = base64.PadRight(base64.Length + padding, '=');

            return Convert.FromBase64String(base64);
        }
    }
}
