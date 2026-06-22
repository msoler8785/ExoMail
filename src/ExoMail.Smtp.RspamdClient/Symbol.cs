using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExoMail.Smtp.RspamdClient
{
    public class Symbol
    {
        [JsonProperty(PropertyName ="name")]
        public string Name { get; set; }
        [JsonProperty(PropertyName = "score")]
        public double Score { get; set; }
        [JsonProperty(PropertyName = "options")]
        public string[] Options { get; set; }

        public override string ToString()
        {
            string options = "None";

            if (Options != null)
                options = String.Join("; ", Options);

            return $"Name: {Name}; Score: {Score}; Options: {options};";
        }
    }
}
