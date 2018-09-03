using System.Collections.Generic;
using System.Text;

namespace QueueIT.QueueToken
{
    internal class EnqueueTokenPayload : IEnqueueTokenPayload
    {
        public string Key { get; }
        public double? Rank { get; }
        private Dictionary<string, string> _customData;

        public EnqueueTokenPayload()
        {
            this._customData = new Dictionary<string, string>();
        }

        public EnqueueTokenPayload(EnqueueTokenPayload payload, string key)
        {
            this.Key = key;
            this.Rank = payload.Rank;
            this._customData = payload._customData;
        }

        public EnqueueTokenPayload(EnqueueTokenPayload payload, double rank)
        {
            this.Key = payload.Key;
            this.Rank = rank;
            this._customData = payload._customData;
        }

        public EnqueueTokenPayload(EnqueueTokenPayload payload, string customDataKey, string customDataValue)
        {
            this.Key = payload.Key;
            this.Rank = payload.Rank;
            this._customData = payload._customData;
            this._customData.Add(customDataKey, customDataValue);
        }

        public string GetCustomDataValue(string key)
        {
            if (!_customData.ContainsKey(key))
                return null;
            return this._customData[key];
        }

        public string Serialize()
        {
            bool addComma = false;

            StringBuilder sb = new StringBuilder();
            sb.Append("{");

            if (this.Rank != null)
            {
                sb.Append("\"r\":");
                sb.Append(this.Rank);
                addComma = true;
            }

            if (this.Key != null)
            {
                if (addComma)
                {
                    sb.Append(",");
                }

                sb.Append("\"k\":\"");
                sb.Append(this.Key.Replace("\"", "\\\""));
                sb.Append("\"");
                addComma = true;
            }

            bool addCustomDataComma = false;
            if (this._customData.Count > 0)
            {
                if (addComma)
                {
                    sb.Append(",");
                }

                sb.Append("\"cd\":{");

                foreach (KeyValuePair<string, string> keyValuePair in _customData)
                {
                    if (addCustomDataComma)
                    {
                        sb.Append(",");
                    }

                    sb.Append("\"");
                    sb.Append(keyValuePair.Key.Replace("\"", "\\\""));
                    sb.Append("\":\"");
                    sb.Append(keyValuePair.Value.Replace("\"", "\\\""));
                    sb.Append("\"");
                    addCustomDataComma = true;

                }

                sb.Append("}");
                addComma = true;
            }

            sb.Append("}");
            return sb.ToString();
        }
    }
}

