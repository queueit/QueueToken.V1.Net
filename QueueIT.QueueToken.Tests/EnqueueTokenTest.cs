using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace QueueIT.QueueToken.Tests
{
    public class EnqueueTokenTest
    {

        [Fact]
        public void Factory_simple()
        {


            DateTime startTime = DateTime.UtcNow;
            string expectedCustomerId = "ticketania";
            IEnqueueToken token = Token
                    .Enqueue(expectedCustomerId)
                    .Generate("5ebbf794-1665-4d48-80d6-21ac34be7faedf9e10b3-551a-4682-bb77-fee59d6355d6");
            DateTime endTime = DateTime.UtcNow;

            Assert.Equal(expectedCustomerId, token.CustomerId);
            Assert.NotNull(token.TokenIdentifier);
            Assert.Equal(TokenVersion.QT1, token.TokenVersion);
            Assert.Equal(EncryptionType.AES256, token.Encryption);
            Assert.True(startTime <= token.Issued);
            Assert.True(endTime >= token.Issued);
            Assert.Equal(token.Expires, DateTime.MaxValue);
            Assert.Null(token.EventId);
            Assert.Null(token.Payload);
        }

        [Fact]
        public void Factory_WithValidity_long()
        {
            long expectedValidity = 3000;

            IEnqueueToken token = Token
                    .Enqueue("ticketania")
                    .WithValidity(expectedValidity)
                    .Generate("5ebbf794-1665-4d48-80d6-21ac34be7faedf9e10b3-551a-4682-bb77-fee59d6355d6");

            Assert.Equal(token.Issued.AddMilliseconds(expectedValidity), token.Expires);
        }

        [Fact]
        public void Factory_WithValidity_date()
        {
            DateTime expectedValidity = new DateTime(2030, 01, 01, 12, 00, 00);

            IEnqueueToken token = Token
                    .Enqueue("ticketania")
                    .WithValidity(expectedValidity)
                    .Generate("5ebbf794-1665-4d48-80d6-21ac34be7faedf9e10b3-551a-4682-bb77-fee59d6355d6");

            Assert.Equal(expectedValidity, token.Expires);
        }
        
        [Fact]
        public void Factory_WithEventId()
        {
            string expectedEventId = "myevent";

            IEnqueueToken token = Token
                        .Enqueue("ticketania")
                        .WithEventId(expectedEventId)
                        .Generate("5ebbf794-1665-4d48-80d6-21ac34be7faedf9e10b3-551a-4682-bb77-fee59d6355d6");

            Assert.Equal(expectedEventId, token.EventId);
        }

        [Fact]
        public void Factory_WithPayload()
        {
            IEnqueueTokenPayload expectedPayload = Payload.Enqueue().WithKey("somekey").Generate();

            IEnqueueToken token = Token
                        .Enqueue("ticketania")
                        .WithPayload(expectedPayload)
                        .Generate("5ebbf794-1665-4d48-80d6-21ac34be7faedf9e10b3-551a-4682-bb77-fee59d6355d6");

            Assert.Equal(expectedPayload, token.Payload);
        }

        [Fact]
        public void Factory_WithPayload_WithKey_WithRank()
        {
            string expectedEventId = "myevent";
            string expectedCustomerId = "ticketania";
                long expectedValidity = 1100;

            IEnqueueTokenPayload expectedPayload = Payload
                    .Enqueue()
                    .WithKey("somekey")
                    .Generate();

            IEnqueueToken token = Token
                        .Enqueue(expectedCustomerId)
                        .WithPayload(expectedPayload)
                        .WithEventId(expectedEventId)
                        .WithValidity(expectedValidity)
                        .Generate("5ebbf794-1665-4d48-80d6-21ac34be7faedf9e10b3-551a-4682-bb77-fee59d6355d6");

            Assert.Equal(expectedCustomerId, token.CustomerId);
            Assert.Equal(expectedEventId, token.EventId);
            Assert.Equal(expectedValidity, (token.Expires - token.Issued).TotalMilliseconds);
            Assert.Equal(expectedPayload, token.Payload);
        }

        [Fact]
        public void GenerateToken_WithPayload()
        {
            string expectedSignedToken = "eyJ0eXAiOiJRVDEiLCJlbmMiOiJBRVMyNTYiLCJpc3MiOjE1MzQ3MjMyMDAwMDAsImV4cCI6MTUzOTEyOTYwMDAwMCwidGkiOmEyMWQ0MjNhLTQzZmQtNDgyMS04NGZhLTQzOTBmNmEyZmQzZSwiYyI6dGlja2V0YW5pYSwiZSI6Im15ZXZlbnQifQ.0rDlI69F1Dx4Twps5qD4cQrbXbCRiezBd6fH1PVm6CnVY456FALkAhN3rgVrh_PGCJHcEXN5zoqFg65MH8WZc_CQdD63hJre3Sedu0-9zIs.pPNGW8yCjx4yM5L1NMx8kEgVdUBMVWB13FfZUSwCuqo";

            IEnqueueTokenPayload payload = Payload
                    .Enqueue()
                    .WithKey("somekey")
                    .WithRank(0.45678663514)
                    .WithCustomData("color", "blue")
                    .WithCustomData("size", "medium")
                    .Generate();

            EnqueueToken token = new EnqueueToken(
                "a21d423a-43fd-4821-84fa-4390f6a2fd3e", 
                "ticketania", 
                "myevent", 
                new DateTime(2018,08,20,0,0,0,DateTimeKind.Utc), 
                new DateTime(2018,10,10,0,0,0,DateTimeKind.Utc), 
                payload);
            token.Generate("5ebbf794-1665-4d48-80d6-21ac34be7faedf9e10b3-551a-4682-bb77-fee59d6355d6", false);

            string actualSignedToken = token.SignedToken;

            Assert.Equal(expectedSignedToken, actualSignedToken);
        }

        [Fact]
        public void GenerateToken_WithoutPayload()
        {
            string expectedSignedToken = "eyJ0eXAiOiJRVDEiLCJlbmMiOiJBRVMyNTYiLCJpc3MiOjE1MzQ3MjMyMDAwMDAsImV4cCI6MTUzOTEyOTYwMDAwMCwidGkiOmEyMWQ0MjNhLTQzZmQtNDgyMS04NGZhLTQzOTBmNmEyZmQzZSwiYyI6dGlja2V0YW5pYSwiZSI6Im15ZXZlbnQifQ..Kzr3hIs4cqxKQyITuC45zLqUycvvEnYK7CbI7uL1zeA";

            IEnqueueTokenPayload payload = Payload
                .Enqueue()
                .WithKey("somekey")
                .WithRank(0.45678663514)
                .WithCustomData("color", "blue")
                .WithCustomData("size", "medium")
                .Generate();

            EnqueueToken token = new EnqueueToken(
                "a21d423a-43fd-4821-84fa-4390f6a2fd3e",
                "ticketania",
                "myevent",
                new DateTime(2018, 08, 20, 0, 0, 0, DateTimeKind.Utc),
                new DateTime(2018, 10, 10, 0, 0, 0, DateTimeKind.Utc),
                null);
            token.Generate("5ebbf794-1665-4d48-80d6-21ac34be7faedf9e10b3-551a-4682-bb77-fee59d6355d6", false);

            string actualSignedToken = token.SignedToken;

            Assert.Equal(expectedSignedToken, actualSignedToken);
        }
    }

}
