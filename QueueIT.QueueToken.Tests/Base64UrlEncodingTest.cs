using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace QueueIT.QueueToken.Tests
{
    public class Base64UrlEncodingTest
    {
        [Theory]
        [InlineData("any carnal pleasure.", "YW55IGNhcm5hbCBwbGVhc3VyZS4")]
        [InlineData("any carnal pleasure", "YW55IGNhcm5hbCBwbGVhc3VyZQ")]
        [InlineData("any carnal pleasur", "YW55IGNhcm5hbCBwbGVhc3Vy")]
        [InlineData("any carnal pleasu", "YW55IGNhcm5hbCBwbGVhc3U")]
        [InlineData("any carnal pleas", "YW55IGNhcm5hbCBwbGVhcw")]
        public void Encode(string input, string expectedOutput)
        {
            var base64url = Base64UrlEncoding.Encode(Encoding.UTF8.GetBytes(input));

            Assert.Equal(expectedOutput, base64url);
        }

        [Theory]
        [InlineData("any carnal pleasure.", "YW55IGNhcm5hbCBwbGVhc3VyZS4")]
        [InlineData("any carnal pleasure", "YW55IGNhcm5hbCBwbGVhc3VyZQ")]
        [InlineData("any carnal pleasur", "YW55IGNhcm5hbCBwbGVhc3Vy")]
        [InlineData("any carnal pleasu", "YW55IGNhcm5hbCBwbGVhc3U")]
        [InlineData("any carnal pleas", "YW55IGNhcm5hbCBwbGVhcw")]
        public void Decode(string expectedOutput, string input)
        {
            var bytes = Base64UrlEncoding.Decode(input);

            Assert.Equal(expectedOutput, Encoding.UTF8.GetString(bytes));
        }

    }
}
