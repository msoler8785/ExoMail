using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExoMail.Smtp.RspamdClient
{
    public class RspamdResult
    {
        [JsonProperty(PropertyName = "default")]
        public DefaultMetric DefaultMetric { get; set; }
    }
}
