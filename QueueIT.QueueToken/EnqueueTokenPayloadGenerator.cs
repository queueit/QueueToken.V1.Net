using QueueIT.QueueToken.Model;
using System;
using System.Collections.Generic;

namespace QueueIT.QueueToken
{
    public class EnqueueTokenPayloadGenerator
    {
        private EnqueueTokenPayload _payload;

        public EnqueueTokenPayloadGenerator()
        {
            this._payload = new EnqueueTokenPayload();
        }

        public EnqueueTokenPayloadGenerator WithKey(string key)
        {
            this._payload = new EnqueueTokenPayload(this._payload, key);

            return this;
        }

        public EnqueueTokenPayloadGenerator WithRelativeQuality(double relativeQuality)
        {
            this._payload = new EnqueueTokenPayload(this._payload, relativeQuality);

            return this;
        }

        public EnqueueTokenPayloadGenerator WithCustomData(string key, string value)
        {
            _payload = new EnqueueTokenPayload(_payload, key, value);
            return this;
        }

        public EnqueueTokenPayloadGenerator WithCustomData(Dictionary<string, string> data)
        {
            _payload = new EnqueueTokenPayload(_payload.Key, _payload.RelativeQuality, data)
            {
                Origin = _payload.Origin
            };
            return this;
        }

        public EnqueueTokenPayloadGenerator WithOrigin(string origin)
        {
            var originEnum = (TokenOrigin)Enum.Parse(typeof(TokenOrigin), origin, true);
            _payload = new EnqueueTokenPayload(_payload, originEnum);

            return this;
        }

        public EnqueueTokenPayloadGenerator WithOrigin(TokenOrigin origin)
        {
            _payload = new EnqueueTokenPayload(_payload, origin);

            return this;
        }


        public IEnqueueTokenPayload Generate()
        {
            return this._payload;
        }
    }
}