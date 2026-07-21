using QueueIT.QueueToken.Model;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace QueueIT.QueueToken.Tests
{
    public class EnqueueTokenPayloadTest
    {
        [Fact]
        public void factory_key()
        {
            string expectedKey = "myKey";
            IEnqueueTokenPayload instance = Payload
                    .Enqueue()
                    .WithKey(expectedKey)
                    .Generate();
            string actualKey = instance.Key;
            var actualCustomData = instance.CustomData;
            Assert.Equal(expectedKey, actualKey);
            Assert.Null(instance.RelativeQuality);
            Assert.NotNull(actualCustomData);
            Assert.Equal(0, actualCustomData.Count);

            Assert.False(instance.CustomData.ContainsKey("key"));
        }

        [Fact]
        public void factory_origin_default_is_connector()
        {
            string expectedKey = "myKey";
            TokenOrigin expectedOrigin = TokenOrigin.Connector;
            IEnqueueTokenPayload instance = Payload
                    .Enqueue()
                    .WithKey(expectedKey)
                    .Generate();
            string actualKey = instance.Key;
            var actualCustomData = instance.CustomData;
            Assert.Equal(expectedKey, actualKey);
            Assert.Null(instance.RelativeQuality);
            Assert.NotNull(actualCustomData);
            Assert.Equal(0, actualCustomData.Count);
            Assert.Equal(expectedOrigin, instance.Origin);

        }

        [Theory]
        [InlineData(TokenOrigin.InviteOnly)]
        [InlineData(TokenOrigin.AkamaiBotManagerHeaderValidator)]
        public void factory_origins(TokenOrigin inputOrigin)
        {
            string expectedKey = "myKey";
            TokenOrigin expectedOrigin = inputOrigin;
            IEnqueueTokenPayload instance = Payload
                    .Enqueue()
                    .WithKey(expectedKey)
                    .WithOrigin(inputOrigin.ToString())
                    .Generate();
            string actualKey = instance.Key;
            var actualCustomData = instance.CustomData;
            Assert.Equal(expectedKey, actualKey);
            Assert.Null(instance.RelativeQuality);
            Assert.NotNull(actualCustomData);
            Assert.Equal(0, actualCustomData.Count);
            Assert.Equal(expectedOrigin, instance.Origin);
        }

        [Fact]
        public void factory_key_relativequality()
        {
            string expectedKey = "myKey";
            Double expectedRelativeQuality = 0.456;
            IEnqueueTokenPayload instance = Payload
                .Enqueue()
                .WithKey(expectedKey)
                .WithRelativeQuality(expectedRelativeQuality)
                .Generate();
            string actualKey = instance.Key;
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
            string expectedKey = "myKey";
            Double expectedRelativeQuality = 0.456;
            string expectedCustomDataValue = "Value";
            IEnqueueTokenPayload instance = Payload
                .Enqueue()
                .WithKey(expectedKey)
                .WithRelativeQuality(expectedRelativeQuality)
                .WithCustomData("key", expectedCustomDataValue)
                .Generate();
            string actualKey = instance.Key;
            Double? actualRelativeQuality = instance.RelativeQuality;
            string actualCustomData = instance.CustomData["key"];
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
            string actualKey = instance.Key;
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
            string expectedCustomDataValue = "Value";
            IEnqueueTokenPayload instance = Payload
                .Enqueue()
                .WithRelativeQuality(expectedRelativeQuality)
                .WithCustomData("key", expectedCustomDataValue)
                .Generate();
            string actualKey = instance.Key;
            Double? actualRelativeQuality = instance.RelativeQuality;
            string actualCustomData = instance.CustomData["key"];
            Assert.Null(actualKey);
            Assert.Equal(expectedRelativeQuality, actualRelativeQuality);
            Assert.Equal(expectedCustomDataValue, actualCustomData);
        }

        [Fact]
        public void factory_customdata()
        {
            var expectedCustomDataValue = "value";
            IEnqueueTokenPayload instance = Payload
                .Enqueue()
                .WithCustomData("key", expectedCustomDataValue)
                .Generate();
            var actualKey = instance.Key;
            var actualRelativeQuality = instance.RelativeQuality;
            var actualCustomData = instance.CustomData["key"];
            Assert.Null(actualKey);
            Assert.Null(actualRelativeQuality);
            Assert.Equal(expectedCustomDataValue, actualCustomData);
        }

        [Fact]
        public void factory_customdataasdictionary()
        {
            var data = new Dictionary<string, string>();
            data.Add("k1", "v1");
            data.Add("k2", "v2");
            IEnqueueTokenPayload instance = Payload
                .Enqueue()
                .WithCustomData(data)
                .Generate();
            var actualKey = instance.Key;
            var actualRelativeQuality = instance.RelativeQuality;            
            Assert.Null(actualKey);
            Assert.Null(actualRelativeQuality);
            Assert.True(instance.CustomData.ContainsKey("k1"));
            Assert.Equal("v1", instance.CustomData["k1"]);
            Assert.True(instance.CustomData.ContainsKey("k2"));
            Assert.Equal("v2", instance.CustomData["k2"]);            
        }

        [Fact]
        public void serialize_key_relativequality_multicustomdata()
        {
            string expectedJson = "{\"r\":0.456,\"k\":\"myKey\",\"cd\":{\"key1\":\"Value1\",\"key2\":\"Value2\",\"key3\":\"Value3\"},\"O\":\"Connector\"}";

            IEnqueueTokenPayload instance = Payload
                .Enqueue()
                .WithKey("myKey")
                .WithRelativeQuality(0.456)
                .WithCustomData("key1", "Value1")
                .WithCustomData("key2", "Value2")
                .WithCustomData("key3", "Value3")
                .Generate();
            string actualJson = Encoding.UTF8.GetString(((EnqueueTokenPayload)instance).Serialize());

            Assert.Equal(expectedJson, actualJson);
        }

        [Fact]
        public void serialize_key_relativequality_onecustomdata()
        {
            string expectedJson = "{\"r\":0.456,\"k\":\"myKey\",\"cd\":{\"key1\":\"Value1\"},\"O\":\"Connector\"}";

            IEnqueueTokenPayload instance = Payload
                .Enqueue()
                .WithKey("myKey")
                .WithRelativeQuality(0.456)
                .WithCustomData("key1", "Value1")
                .Generate();
            string actualJson = Encoding.UTF8.GetString(((EnqueueTokenPayload)instance).Serialize());

            Assert.Equal(expectedJson, actualJson);
        }

        [Fact]
        public void serialize_key_relativequality()
        {
            string expectedJson = "{\"r\":0.456,\"k\":\"myKey\",\"O\":\"Connector\"}";

            IEnqueueTokenPayload instance = Payload
                .Enqueue()
                .WithKey("myKey")
                .WithRelativeQuality(0.456)
                .Generate();
            string actualJson = Encoding.UTF8.GetString(((EnqueueTokenPayload)instance).Serialize());

            Assert.Equal(expectedJson, actualJson);
        }

        [Fact]
        public void serialize_key()
        {
            string expectedJson = "{\"k\":\"myKey\",\"O\":\"Connector\"}";

            IEnqueueTokenPayload instance = Payload
                .Enqueue()
                .WithKey("myKey")
                .Generate();
            string actualJson = Encoding.UTF8.GetString(((EnqueueTokenPayload)instance).Serialize());

            Assert.Equal(expectedJson, actualJson);
        }

        [Fact]
        public void serialize_key_escaped()
        {
            string expectedJson = "{\"k\":\"my\\\"Key\",\"O\":\"Connector\"}";

            IEnqueueTokenPayload instance = Payload
                .Enqueue()
                .WithKey("my\"Key")
                .Generate();
            string actualJson = Encoding.UTF8.GetString(((EnqueueTokenPayload)instance).Serialize());

            Assert.Equal(expectedJson, actualJson);
        }

        [Fact]
        public void serialize_relativequality()
        {
            string expectedJson = "{\"r\":0.456,\"O\":\"Connector\"}";

            IEnqueueTokenPayload instance = Payload
                .Enqueue()
                .WithRelativeQuality(0.456)
                .Generate();
            string actualJson = Encoding.UTF8.GetString(((EnqueueTokenPayload)instance).Serialize());

            Assert.Equal(expectedJson, actualJson);
        }

        [Fact]
        public void serialize_customdata()
        {
            string expectedJson = "{\"cd\":{\"key1\":\"Value1\"},\"O\":\"Connector\"}";

            IEnqueueTokenPayload instance = Payload
                .Enqueue()
                .WithCustomData("key1", "Value1")
                .Generate();
            string actualJson = Encoding.UTF8.GetString(((EnqueueTokenPayload)instance).Serialize());

            Assert.Equal(expectedJson, actualJson);
        }

        [Fact]
        public void serialize_customdata_escaped()
        {
            string expectedJson = "{\"cd\":{\"ke\\\"y1\":\"Va\\\"lue1\"},\"O\":\"Connector\"}";

            IEnqueueTokenPayload instance = Payload
                .Enqueue()
                .WithCustomData("ke\"y1", "Va\"lue1")
                .Generate();
            string actualJson = Encoding.UTF8.GetString(((EnqueueTokenPayload)instance).Serialize());

            Assert.Equal(expectedJson, actualJson);
        }
    }
}
