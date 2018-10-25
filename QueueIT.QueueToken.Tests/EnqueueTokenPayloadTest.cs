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
            var actualCustomData = instance.CustomData;
            Assert.Equal(expectedKey, actualKey);
            Assert.Null(instance.RelativeQuality);
            Assert.NotNull(actualCustomData);
            Assert.Equal(0, actualCustomData.Count);

            Assert.False(instance.CustomData.ContainsKey("key"));
        }

        [Fact]
        public void factory_key_relativequality()
        {
            String expectedKey = "myKey";
            Double expectedRelativeQuality = 0.456;
            IEnqueueTokenPayload instance = Payload
                .Enqueue()
                .WithKey(expectedKey)
                .WithRelativeQuality(expectedRelativeQuality)
                .Generate();
            String actualKey = instance.Key;
            Double? actualRelativeQuality = instance.RelativeQuality;
            var actualCustomData = instance.CustomData;
            Assert.Equal(expectedKey, actualKey);
            Assert.Equal(expectedRelativeQuality, actualRelativeQuality);
            Assert.NotNull(actualCustomData);
            Assert.Equal(0, actualCustomData.Count);
            Assert.False(instance.CustomData.ContainsKey("key"));
        }

        [Fact]
        public void factory_key_relativequality_customdata()
        {
            String expectedKey = "myKey";
            Double expectedRelativeQuality = 0.456;
            String expectedCustomDataValue = "Value";
            IEnqueueTokenPayload instance = Payload
                .Enqueue()
                .WithKey(expectedKey)
                .WithRelativeQuality(expectedRelativeQuality)
                .WithCustomData("key", expectedCustomDataValue)
                .Generate();
            String actualKey = instance.Key;
            Double? actualRelativeQuality = instance.RelativeQuality;
            String actualCustomData = instance.CustomData["key"];
            Assert.Equal(expectedKey, actualKey);
            Assert.Equal(expectedRelativeQuality, actualRelativeQuality);
            Assert.Equal(expectedCustomDataValue, actualCustomData);
        }

        [Fact]
        public void factory_relativequality()
        {
            Double expectedRelativeQuality = 0.456;
            IEnqueueTokenPayload instance = Payload
                .Enqueue()
                .WithRelativeQuality(expectedRelativeQuality)
                .Generate();
            String actualKey = instance.Key;
            Double? actualRelativeQuality = instance.RelativeQuality;
            var actualCustomData = instance.CustomData;
            Assert.Null(actualKey);
            Assert.Equal(expectedRelativeQuality, actualRelativeQuality);
            Assert.NotNull(actualCustomData);
            Assert.Equal(0, actualCustomData.Count);
        }

        [Fact]
        public void factory_relativequality_customdata()
        {
            Double expectedRelativeQuality = 0.456;
            String expectedCustomDataValue = "Value";
            IEnqueueTokenPayload instance = Payload
                .Enqueue()
                .WithRelativeQuality(expectedRelativeQuality)
                .WithCustomData("key", expectedCustomDataValue)
                .Generate();
            String actualKey = instance.Key;
            Double? actualRelativeQuality = instance.RelativeQuality;
            String actualCustomData = instance.CustomData["key"];
            Assert.Null(actualKey);
            Assert.Equal(expectedRelativeQuality, actualRelativeQuality);
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
            Double? actualRelativeQuality = instance.RelativeQuality;
            String actualCustomData = instance.CustomData["key"];
            Assert.Null(actualKey);
            Assert.Null(actualRelativeQuality);
            Assert.Equal(expectedCustomDataValue, actualCustomData);
        }

        [Fact]
        public void serialize_key_relativequality_multicustomdata()
        {
            String expectedJson = "{\"r\":0.456,\"k\":\"myKey\",\"cd\":{\"key1\":\"Value1\",\"key2\":\"Value2\",\"key3\":\"Value3\"}}";

            IEnqueueTokenPayload instance = Payload
                .Enqueue()
                .WithKey("myKey")
                .WithRelativeQuality(0.456)
                .WithCustomData("key1", "Value1")
                .WithCustomData("key2", "Value2")
                .WithCustomData("key3", "Value3")
                .Generate();
            String actualJson = Encoding.UTF8.GetString(((EnqueueTokenPayload)instance).Serialize());

            Assert.Equal(expectedJson, actualJson);
        }

        [Fact]
        public void serialize_key_relativequality_onecustomdata()
        {
            String expectedJson = "{\"r\":0.456,\"k\":\"myKey\",\"cd\":{\"key1\":\"Value1\"}}";

            IEnqueueTokenPayload instance = Payload
                .Enqueue()
                .WithKey("myKey")
                .WithRelativeQuality(0.456)
                .WithCustomData("key1", "Value1")
                .Generate();
            String actualJson = Encoding.UTF8.GetString(((EnqueueTokenPayload)instance).Serialize());

            Assert.Equal(expectedJson, actualJson);
        }

        [Fact]
        public void serialize_key_relativequality()
        {
            String expectedJson = "{\"r\":0.456,\"k\":\"myKey\"}";

            IEnqueueTokenPayload instance = Payload
                .Enqueue()
                .WithKey("myKey")
                .WithRelativeQuality(0.456)
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
        public void serialize_relativequality()
        {
            String expectedJson = "{\"r\":0.456}";

            IEnqueueTokenPayload instance = Payload
                .Enqueue()
                .WithRelativeQuality(0.456)
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
