using Discord.Commands;

using DiscordBotCS.HttpResponseTemplates.WebsterDictionaryResponse;

namespace DiscordBotCS.Commands
{
    public class DictionaryModule : ModuleBase<SocketCommandContext>
    {
        // This url is the address for the Merriam-Webster collegiate dictionary api. When we send a request to this url,
        // it will send back a response with the information we want, similarly to how the browser would get a webpage.
        private static string url = @"https://www.dictionaryapi.com/api/v3/references/collegiate/json/[WORD]?key=[APIKEY]";

        [Command("def")]
        public async Task DefinitionAsync([Remainder]string word)
        {
            await Task.Run(async () =>
            {
                using var typing = this.Context.Channel.EnterTypingState();

                var apiKey = DiscordBot.Credentials.DictionaryApiKey;
                // Plug requested word and api-key into url
                var requestUrl = url.Replace("[WORD]", word).Replace("[APIKEY]", apiKey);

                // Send request to API and receive the JSON response
                var response = await DiscordBot.HttpClient.GetAsync(requestUrl);
                try
                {
                    response.EnsureSuccessStatusCode();
                }
                catch (HttpRequestException)
                {
                    await ReplyAsync($"An error occurred. The word might not exist on Merriam-Webster.");
                    return;
                }

                string json = await response.Content.ReadAsStringAsync();
                if (string.IsNullOrWhiteSpace(json)) return;

                try
                {
                    var apiResponse = WebsterDictionaryResponse.GetWebsterDictionaryData(json);

                    if (apiResponse == null)
                    {
                        await ReplyAsync("An error occured while processing your request");
                        return;
                    }

                    // Only show up to 5 of the most relevant definitions
                    var relevantWords = apiResponse.Take(5);
                    var wordsAndDefinitions = relevantWords.Select((word, index) => $"{index + 1}) {word.Metadata.id} - {string.Join(";\n\t- ", word.Definitions)}");
                    await ReplyAsync(string.Join('\n', wordsAndDefinitions));
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    await ReplyAsync($"An error occured while processing your request:\n {ex.Message}");
                    return;
                }
            });
        }
    }
}
