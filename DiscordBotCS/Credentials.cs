using System.Text.Json;
using System.Text.Json.Serialization;

namespace DiscordBotCS
{
    public class Credentials
    {
        [JsonPropertyName("bot-token")]
        public string Token { get; set; } = string.Empty;
        [JsonPropertyName("dictionary-api-key")]
        public string DictionaryApiKey { get; set; } = string.Empty;
        [JsonPropertyName("thesaurus-api-key")]
        public string ThesaurusApiKey { get; set; } = string.Empty;
        [JsonPropertyName("connection-string")]
        public string ConnectionString { get; set; } = string.Empty;
        [JsonPropertyName("youtube-api-key")]
        public string YoutubeApiKey { get; set; } = string.Empty;

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
