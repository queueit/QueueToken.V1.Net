using System;
using System.Text;
using Xunit;

namespace QueueIT.QueueToken.Tests
{
    public class EnqueueTokenPayloadTest
    {
        [Fact]
        public void factory_key()
        {
            String expectedKey = "myKey";
            IEnqueueTokenPayload instance = Payload
                    .Enqueue()
                    .WithKey(expectedKey)
                    .Generate();
            String actualKey = instance.Key;
            Assert.Equal(expectedKey, actualKey);
            Assert.Null(instance.Rank);
            Assert.False(instance.GetCustomDataDictionary().ContainsKey("key"));
        }

        [Fact]
        public void factory_key_rank()
        {
            String expectedKey = "myKey";
            Double expectedRank = 0.456;
            IEnqueueTokenPayload instance = Payload
                .Enqueue()
                .WithKey(expectedKey)
                .WithRank(expectedRank)
                .Generate();
            String actualKey = instance.Key;
            Double? actualRank = instance.Rank;
            Assert.Equal(expectedKey, actualKey);
            Assert.Equal(expectedRank, actualRank);
            Assert.False(instance.GetCustomDataDictionary().ContainsKey("key"));
        }

        [Fact]
        public void factory_key_rank_customdata()
        {
            String expectedKey = "myKey";
            Double expectedRank = 0.456;
            String expectedCustomDataValue = "Value";
            IEnqueueTokenPayload instance = Payload
                .Enqueue()
                .WithKey(expectedKey)
                .WithRank(expectedRank)
                .WithCustomData("key", expectedCustomDataValue)
                .Generate();
            String actualKey = instance.Key;
            Double? actualRank = instance.Rank;
            String actualCustomData = instance.GetCustomDataDictionary()["key"];
            Assert.Equal(expectedKey, actualKey);
            Assert.Equal(expectedRank, actualRank);
            Assert.Equal(expectedCustomDataValue, actualCustomData);
        }

        [Fact]
        public void factory_rank()
        {
            Double expectedRank = 0.456;
            IEnqueueTokenPayload instance = Payload
                .Enqueue()
                .WithRank(expectedRank)
                .Generate();
            String actualKey = instance.Key;
            Double? actualRank = instance.Rank;
            Assert.Null(actualKey);
            Assert.Equal(expectedRank, actualRank);
            Assert.False(instance.GetCustomDataDictionary().ContainsKey("key"));
        }

        [Fact]
        public void factory_rank_customdata()
        {
            Double expectedRank = 0.456;
            String expectedCustomDataValue = "Value";
            IEnqueueTokenPayload instance = Payload
                .Enqueue()
                .WithRank(expectedRank)
                .WithCustomData("key", expectedCustomDataValue)
                .Generate();
            String actualKey = instance.Key;
            Double? actualRank = instance.Rank;
            String actualCustomData = instance.GetCustomDataDictionary()["key"];
            Assert.Null(actualKey);
            Assert.Equal(expectedRank, actualRank);
            Assert.Equal(expectedCustomDataValue, actualCustomData);
        }

        [Fact]
        public void factory_customdata()
        {
            String expectedCustomDataValue = "value";
            IEnqueueTokenPayload instance = Payload
                .Enqueue()
                .WithCustomData("key", expectedCustomDataValue)
                .Generate();
            String actualKey = instance.Key;
            Double? actualRank = instance.Rank;
            String actualCustomData = instance.GetCustomDataDictionary()["key"];
            Assert.Null(actualKey);
            Assert.Null(actualRank);
            Assert.Equal(expectedCustomDataValue, actualCustomData);
        }

        [Fact]
        public void serialize_key_rank_multicustomdata()
        {
            String expectedJson = "{\"r\":0.456,\"k\":\"myKey\",\"cd\":{\"key1\":\"Value1\",\"key2\":\"Value2\",\"key3\":\"Value3\"}}";

            IEnqueueTokenPayload instance = Payload
                .Enqueue()
                .WithKey("myKey")
                .WithRank(0.456)
                .WithCustomData("key1", "Value1")
                .WithCustomData("key2", "Value2")
                .WithCustomData("key3", "Value3")
                .Generate();
            String actualJson = Encoding.UTF8.GetString(((EnqueueTokenPayload)instance).Serialize());

            Assert.Equal(expectedJson, actualJson);
        }

        [Fact]
        public void serialize_key_rank_onecustomdata()
        {
            String expectedJson = "{\"r\":0.456,\"k\":\"myKey\",\"cd\":{\"key1\":\"Value1\"}}";

            IEnqueueTokenPayload instance = Payload
                .Enqueue()
                .WithKey("myKey")
                .WithRank(0.456)
                .WithCustomData("key1", "Value1")
                .Generate();
            String actualJson = Encoding.UTF8.GetString(((EnqueueTokenPayload)instance).Serialize());

            Assert.Equal(expectedJson, actualJson);
        }

        [Fact]
        public void serialize_key_rank()
        {
            String expectedJson = "{\"r\":0.456,\"k\":\"myKey\"}";

            IEnqueueTokenPayload instance = Payload
                .Enqueue()
                .WithKey("myKey")
                .WithRank(0.456)
                .Generate();
            String actualJson = Encoding.UTF8.GetString(((EnqueueTokenPayload)instance).Serialize());

            Assert.Equal(expectedJson, actualJson);
        }

        [Fact]
        public void serialize_key()
        {
            String expectedJson = "{\"k\":\"myKey\"}";

            IEnqueueTokenPayload instance = Payload
                .Enqueue()
                .WithKey("myKey")
                .Generate();
            String actualJson = Encoding.UTF8.GetString(((EnqueueTokenPayload)instance).Serialize());

            Assert.Equal(expectedJson, actualJson);
        }

        [Fact]
        public void serialize_key_escaped()
        {
            String expectedJson = "{\"k\":\"my\\\"Key\"}";

            IEnqueueTokenPayload instance = Payload
                .Enqueue()
                .WithKey("my\"Key")
                .Generate();
            String actualJson = Encoding.UTF8.GetString(((EnqueueTokenPayload)instance).Serialize());

            Assert.Equal(expectedJson, actualJson);
        }

        [Fact]
        public void serialize_rank()
        {
            String expectedJson = "{\"r\":0.456}";

            IEnqueueTokenPayload instance = Payload
                .Enqueue()
                .WithRank(0.456)
                .Generate();
            String actualJson = Encoding.UTF8.GetString(((EnqueueTokenPayload)instance).Serialize());

            Assert.Equal(expectedJson, actualJson);
        }

        [Fact]
        public void serialize_customdata()
        {
            String expectedJson = "{\"cd\":{\"key1\":\"Value1\"}}";

            IEnqueueTokenPayload instance = Payload
                .Enqueue()
                .WithCustomData("key1", "Value1")
                .Generate();
            String actualJson = Encoding.UTF8.GetString(((EnqueueTokenPayload)instance).Serialize());

            Assert.Equal(expectedJson, actualJson);
        }

        [Fact]
        public void serialize_customdata_escaped()
        {
            String expectedJson = "{\"cd\":{\"ke\\\"y1\":\"Va\\\"lue1\"}}";

            IEnqueueTokenPayload instance = Payload
                .Enqueue()
                .WithCustomData("ke\"y1", "Va\"lue1")
                .Generate();
            String actualJson = Encoding.UTF8.GetString(((EnqueueTokenPayload)instance).Serialize());

            Assert.Equal(expectedJson, actualJson);
        }
    }
}
