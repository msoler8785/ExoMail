using ExoMail.Smtp.Common.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ExoMail.Smtp.RspamdClient
{
    public class RspamdClient
    {

        private readonly HttpClient _httpClient;
        private readonly string _requestUri;
        private readonly JsonSerializer _jsonSerializer;

        public RspamdClient(string uri)
        {
            _httpClient = new HttpClient();
            _requestUri = uri;
            _jsonSerializer = new JsonSerializer();
        }

        public async Task<RspamdResult> ScoreMessageAsync(IQueuedMessage queuedMessage)
        {
            using (var content = new StreamContent(await queuedMessage.MessageData.GetMessagStreamAsync()))
            {
                var request = new HttpRequestMessage(HttpMethod.Post, _requestUri);
                request.Headers.Add("From", queuedMessage.Sender.FullAddress);
                request.Headers.Add("Queue-Id", queuedMessage.Envelopes.FirstOrDefault()?.MessageId);
                request.Content = content;
                var response = await _httpClient.SendAsync(request);

                RspamdResult rspamdResult = null;
                string result = await response.Content.ReadAsStringAsync();
                using (StreamReader sr = new StreamReader(await response.Content.ReadAsStreamAsync()))
                using (JsonReader reader = new JsonTextReader(sr))
                {
                    try
                    {
                        rspamdResult = _jsonSerializer.Deserialize<RspamdResult>(reader);
                    }
                    catch (Exception ex)
                    {

                    }
                }

                return rspamdResult;
            }
        }
    }
}
