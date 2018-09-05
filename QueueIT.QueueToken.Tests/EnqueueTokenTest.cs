using QueueIT.QueueToken.Model;
using System;
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
            string expectedSignedToken =
                "eyJ0eXAiOiJRVDEiLCJlbmMiOiJBRVMyNTYiLCJpc3MiOjE1MzQ3MjMyMDAwMDAsImV4cCI6MTUzOTEyOTYwMDAwMCwidGkiOiJhMjFkNDIzYS00M2ZkLTQ4MjEtODRmYS00MzkwZjZhMmZkM2UiLCJjIjoidGlja2V0YW5pYSIsImUiOiJteWV2ZW50In0.0rDlI69F1Dx4Twps5qD4cQrbXbCRiezBd6fH1PVm6CnVY456FALkAhN3rgVrh_PGCJHcEXN5zoqFg65MH8WZc_CQdD63hJre3Sedu0-9zIs.aZgzkJm57etFaXjjME_-9LjOgPNTTqkp1aJ057HuEiU";

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
                payload);
            token.Generate("5ebbf794-1665-4d48-80d6-21ac34be7faedf9e10b3-551a-4682-bb77-fee59d6355d6", false);

            string actualSignedToken = token.SignedToken;

            Assert.Equal(expectedSignedToken, actualSignedToken);
        }

        [Fact]
        public void GenerateToken_WithoutPayload()
        {
            string expectedSignedToken =
                "eyJ0eXAiOiJRVDEiLCJlbmMiOiJBRVMyNTYiLCJpc3MiOjE1MzQ3MjMyMDAwMDAsImV4cCI6MTUzOTEyOTYwMDAwMCwidGkiOiJhMjFkNDIzYS00M2ZkLTQ4MjEtODRmYS00MzkwZjZhMmZkM2UiLCJjIjoidGlja2V0YW5pYSIsImUiOiJteWV2ZW50In0..nN4Q5wIYKktChsK1_UEuP_tjiZj9xYOd65iYv4E9hbY";

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

        [Fact]
        public void GenerateToken_MinimalHeader()
        {
            string expectedSignedToken =
                "eyJ0eXAiOiJRVDEiLCJlbmMiOiJBRVMyNTYiLCJpc3MiOjE1MzQ3MjMyMDAwMDAsInRpIjoiYTIxZDQyM2EtNDNmZC00ODIxLTg0ZmEtNDM5MGY2YTJmZDNlIiwiYyI6InRpY2tldGFuaWEifQ..ChCRF4bTbt4zlOcvXLjQYouhgqgiNNNZqcci8VWoZIU";

            EnqueueToken token = new EnqueueToken(
                "a21d423a-43fd-4821-84fa-4390f6a2fd3e",
                "ticketania",
                null,
                new DateTime(2018, 08, 20, 0, 0, 0, DateTimeKind.Utc),
                null,
                null);
            token.Generate("5ebbf794-1665-4d48-80d6-21ac34be7faedf9e10b3-551a-4682-bb77-fee59d6355d6", false);

            string actualSignedToken = token.SignedToken;

            Assert.Equal(expectedSignedToken, actualSignedToken);
        }

        [Fact]
        public void Parse_WithoutPayload()
        {
            string signature = "nN4Q5wIYKktChsK1_UEuP_tjiZj9xYOd65iYv4E9hbY";
            string token =
                "eyJ0eXAiOiJRVDEiLCJlbmMiOiJBRVMyNTYiLCJpc3MiOjE1MzQ3MjMyMDAwMDAsImV4cCI6MTUzOTEyOTYwMDAwMCwidGkiOiJhMjFkNDIzYS00M2ZkLTQ4MjEtODRmYS00MzkwZjZhMmZkM2UiLCJjIjoidGlja2V0YW5pYSIsImUiOiJteWV2ZW50In0.";
            string tokenString = token + "." + signature;

            var enqueueToken = EnqueueToken.Parse(tokenString, "5ebbf794-1665-4d48-80d6-21ac34be7faedf9e10b3-551a-4682-bb77-fee59d6355d6");

            Assert.Equal("a21d423a-43fd-4821-84fa-4390f6a2fd3e", enqueueToken.TokenIdentifier);
            Assert.Equal("ticketania", enqueueToken.CustomerId);
            Assert.Equal("myevent", enqueueToken.EventId);
            Assert.Equal(new DateTime(2018, 10, 10, 0, 0, 0, DateTimeKind.Utc), enqueueToken.Expires);
            Assert.Equal(new DateTime(2018, 08, 20, 0, 0, 0, DateTimeKind.Utc), enqueueToken.Issued);
            Assert.Equal(signature, enqueueToken.Signature);
            Assert.Equal(token, enqueueToken.Token);
            Assert.Equal(tokenString, enqueueToken.SignedToken);
            Assert.Equal(EncryptionType.AES256, enqueueToken.Encryption);
            Assert.Equal(TokenVersion.QT1, enqueueToken.TokenVersion);
            Assert.Null(enqueueToken.Payload);
        }

        [Fact]
        public void Parse_WithPayload()
        {
            string signature = "aZgzkJm57etFaXjjME_-9LjOgPNTTqkp1aJ057HuEiU";
            string token =
                "eyJ0eXAiOiJRVDEiLCJlbmMiOiJBRVMyNTYiLCJpc3MiOjE1MzQ3MjMyMDAwMDAsImV4cCI6MTUzOTEyOTYwMDAwMCwidGkiOiJhMjFkNDIzYS00M2ZkLTQ4MjEtODRmYS00MzkwZjZhMmZkM2UiLCJjIjoidGlja2V0YW5pYSIsImUiOiJteWV2ZW50In0.0rDlI69F1Dx4Twps5qD4cQrbXbCRiezBd6fH1PVm6CnVY456FALkAhN3rgVrh_PGCJHcEXN5zoqFg65MH8WZc_CQdD63hJre3Sedu0-9zIs";
            string tokenString = token + "." + signature;

            var enqueueToken = EnqueueToken.Parse(tokenString, "5ebbf794-1665-4d48-80d6-21ac34be7faedf9e10b3-551a-4682-bb77-fee59d6355d6");

            Assert.Equal("a21d423a-43fd-4821-84fa-4390f6a2fd3e", enqueueToken.TokenIdentifier);
            Assert.Equal("ticketania", enqueueToken.CustomerId);
            Assert.Equal("myevent", enqueueToken.EventId);
            Assert.Equal(new DateTime(2018, 10, 10, 0, 0, 0, DateTimeKind.Utc), enqueueToken.Expires);
            Assert.Equal(new DateTime(2018, 08, 20, 0, 0, 0, DateTimeKind.Utc), enqueueToken.Issued);
            Assert.Equal(signature, enqueueToken.Signature);
            Assert.Equal(token, enqueueToken.Token);
            Assert.Equal(tokenString, enqueueToken.SignedToken);
            Assert.Equal(EncryptionType.AES256, enqueueToken.Encryption);
            Assert.Equal(TokenVersion.QT1, enqueueToken.TokenVersion);
            Assert.Equal("somekey", enqueueToken.Payload.Key);
            Assert.Equal(0.45678663514, enqueueToken.Payload.Rank);
            Assert.Equal("blue", enqueueToken.Payload.GetCustomDataValue("color"));
            Assert.Equal("medium", enqueueToken.Payload.GetCustomDataValue("size"));
        }

        [Fact]
        public void Parse_MinimalHeader()
        {
            string signature = "ChCRF4bTbt4zlOcvXLjQYouhgqgiNNNZqcci8VWoZIU";
            string token =
                "eyJ0eXAiOiJRVDEiLCJlbmMiOiJBRVMyNTYiLCJpc3MiOjE1MzQ3MjMyMDAwMDAsInRpIjoiYTIxZDQyM2EtNDNmZC00ODIxLTg0ZmEtNDM5MGY2YTJmZDNlIiwiYyI6InRpY2tldGFuaWEifQ.";
            string tokenString = token + "." + signature;

            var enqueueToken = EnqueueToken.Parse(tokenString, "5ebbf794-1665-4d48-80d6-21ac34be7faedf9e10b3-551a-4682-bb77-fee59d6355d6");

            Assert.Equal("a21d423a-43fd-4821-84fa-4390f6a2fd3e", enqueueToken.TokenIdentifier);
            Assert.Equal("ticketania", enqueueToken.CustomerId);
            Assert.Null(enqueueToken.EventId);
            Assert.Equal(DateTime.MaxValue, enqueueToken.Expires);
            Assert.Equal(new DateTime(2018, 08, 20, 0, 0, 0, DateTimeKind.Utc), enqueueToken.Issued);
            Assert.Equal(signature, enqueueToken.Signature);
            Assert.Equal(token, enqueueToken.Token);
            Assert.Equal(tokenString, enqueueToken.SignedToken);
            Assert.Equal(EncryptionType.AES256, enqueueToken.Encryption);
            Assert.Equal(TokenVersion.QT1, enqueueToken.TokenVersion);
            Assert.Null(enqueueToken.Payload);
        }

    }
}
