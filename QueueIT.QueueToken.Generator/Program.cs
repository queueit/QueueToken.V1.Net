using System;
using System.IO;
using System.Linq;

namespace QueueIT.QueueToken.Generator
{
    class Program
    {
        static void Main(string[] args)
        {
            var tokenCount = int.Parse(args[0]);
            var customerId = args[1];
            var secretKey = args[2];
            var withRelativeQuality = args.Contains("-r");

            var rdn = new Random(DateTime.Now.Millisecond);

            using (var writer = File.CreateText("tokens.txt"))
            {
                for (int i = 0; i < tokenCount; i++)
                {
                    var token = Token.Enqueue(customerId);
                    if (withRelativeQuality)
                        token.WithPayload(Payload.Enqueue().WithRelativeQuality(rdn.NextDouble()).Generate());
                    
                    writer.WriteLine(token.Generate(secretKey).Token);
                }
            }
        }
    }
}
