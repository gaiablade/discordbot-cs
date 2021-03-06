using System.Text.Json.Serialization;
using System.Text.Json;

namespace DiscordBotCS.HttpResponseTemplates.WebsterDictionaryResponse
{
    // JsonSerializer will use this template to parse the json data from the dictionary API
    public class Meta
    {
        public string id { get; set; }
        public string uuid { get; set; }
        public string sort { get; set; }
        public string src { get; set; }
        public string section { get; set; }
        public List<string> stems { get; set; }
        public bool offensive { get; set; }
    }

    public class WebsterDictionaryResponse
    {
        [JsonPropertyName("meta")]
        public Meta Metadata { get; set; }
        [JsonPropertyName("shortdef")]
        public List<string> Definitions { get; set; }
        public string fl { get; set; }
        public string date { get; set; }

        public static WebsterDictionaryResponse[] GetWebsterDictionaryData(string json)
        {
            var data = JsonSerializer.Deserialize<WebsterDictionaryResponse[]>(json);
            return data ?? new WebsterDictionaryResponse[] { };
        }
    }
}