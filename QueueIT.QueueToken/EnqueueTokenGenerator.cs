using System;
using System.Collections.Generic;
using System.Text;

namespace QueueIT.QueueToken
{
    public class EnqueueTokenGenerator
    {
        private EnqueueToken _token;

        public EnqueueTokenGenerator(String customerId)
        {
            this._token = new EnqueueToken(customerId);
        }

        public EnqueueTokenGenerator WithEventId(String eventId)
        {
            this._token = new EnqueueToken(this._token, eventId);

            return this;
        }

        public EnqueueTokenGenerator WithValidity(long validityMillis)
        {
            this._token = new EnqueueToken(this._token, this._token.Issued.AddMilliseconds(validityMillis));

            return this;
        }

        public EnqueueTokenGenerator WithValidity(DateTime validity)
        {
            this._token = new EnqueueToken(this._token, validity);

            return this;
        }

        public EnqueueTokenGenerator WithPayload(IEnqueueTokenPayload payload)
        {
            this._token = new EnqueueToken(this._token, payload);

            return this;
        }

        public IEnqueueToken Generate(String secretKey)
        {
            _token.Generate(secretKey);
            return _token;
        }
    }
}
