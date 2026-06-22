using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;

namespace ExoMail.Smtp.RspamdClient
{
    
    public class DefaultMetric
    {
        [JsonProperty(PropertyName = "is_spam")]
        public bool IsSpam { get; set; }

        [JsonProperty(PropertyName = "is_skipped")]
        public bool IsSkipped { get; set; }

        [JsonProperty(PropertyName = "score")]
        public double Score { get; set; }

        [JsonProperty(PropertyName = "required_score")]
        public double RequiredScore { get; set; }

        [JsonProperty(PropertyName = "action")]
        public string Action { get; set; }

        [JsonExtensionData]
        private IDictionary<string, JToken> _symbols;

        [JsonIgnore]
        public IEnumerable<Symbol> Symbols => _symbols.Select(x => x.Value.ToObject<Symbol>());
    }
}