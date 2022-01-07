using Discord.Commands;

using DiscordBotCS.HttpResponseTemplates.WebsterThesaurusResponse;

namespace DiscordBotCS.Commands
{
    public class ThesaurusModule : ModuleBase<SocketCommandContext>
    {
        private static string url = @"https://www.dictionaryapi.com/api/v3/references/thesaurus/json/[WORD]?key=[APIKEY]";
        [Command("syn")]
        public async Task ThesaurusAsync([Remainder]string word)
        {
            await Task.Run(async () =>
            {
                // This function works very similarly to the one in DictionaryModule.cs, look there
                // for more useful comments
                using var typing = this.Context.Channel.EnterTypingState();

                var apiKey = DiscordBot.Credentials.ThesaurusApiKey;
                var requestUrl = url.Replace("[WORD]", word).Replace("[APIKEY]", apiKey).Trim();

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
                    var apiResponse = WebsterThesaurusResponse.GetSynonyms(json);

                    if (apiResponse == null)
                    {
                        await ReplyAsync("An error occured while processing your request");
                        return;
                    }

                    var bestResponse = apiResponse.First();
                    var synonymsList = bestResponse.meta.syns
                        .Take(4) // consider only 4 of the individual definitions of the word
                        .Select(list => list.Take(2)) // Remove all but two of synonyms from each list
                        .SelectMany(list => list, (list, str) => str) // select the strings of each list
                        .ToArray();

                    var synonyms = $"Synonyms of {bestResponse.meta.id}\n* " + string.Join("\n* ", synonymsList);
                    await ReplyAsync(synonyms ?? String.Empty);
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
