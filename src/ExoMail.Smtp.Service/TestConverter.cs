using ExoMail.Smtp.Common.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExoMail.Smtp.Service
{
    public class TestConverter : JsonConverter
    {
        private readonly IVirtualServerRepository _userManager;

        public TestConverter(IVirtualServerRepository userManager)
        {
            _userManager = userManager;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(EmailUser);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var item = serializer.Deserialize<EmailUser>(reader);

            
            return item;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }
    }
}
