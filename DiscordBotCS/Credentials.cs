using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DiscordBotCS
{
    public class Credentials
    {
        [JsonPropertyName("bot-token")]
        public string token { get; set; } = string.Empty;
        [JsonPropertyName("dictionary-api-key")]
        public string dictionaryApiKey { get; set; } = string.Empty;
        [JsonPropertyName("thesaurus-api-key")]
        public string thesaurusApiKey { get; set; } = string.Empty;

        public static Credentials GetCredentials(string path = "credentials.json")
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException();
            }

            string jsonData = File.ReadAllText(path);
            Credentials? credentials = JsonSerializer.Deserialize<Credentials>(jsonData);
            if (credentials == null)
            {
                throw new Exception($"Error deserializing credentials from {path}");
            }
            return credentials;
        }
    }
}
