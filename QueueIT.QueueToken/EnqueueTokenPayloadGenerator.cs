using System;


namespace QueueIT.QueueToken
{
    public class EnqueueTokenPayloadGenerator
    {
        private EnqueueTokenPayload _payload;

        public EnqueueTokenPayloadGenerator()
        {
            this._payload = new EnqueueTokenPayload();
        }

        public EnqueueTokenPayloadGenerator WithKey(String key)
        {
            this._payload = new EnqueueTokenPayload(this._payload, key);

            return this;
        }

        public EnqueueTokenPayloadGenerator WithRank(double rank)
        {
            this._payload = new EnqueueTokenPayload(this._payload, rank);

            return this;
        }

        public EnqueueTokenPayloadGenerator WithCustomData(String key, String value)
        {
            this._payload = new EnqueueTokenPayload(this._payload, key, value);

            return this;
        }

        public IEnqueueTokenPayload Generate()
        {
            return this._payload;
        }
    }
}
