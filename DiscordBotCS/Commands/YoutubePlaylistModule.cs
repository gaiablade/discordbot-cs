using System.Text.RegularExpressions;

using Discord.Commands;

using DiscordBotCS.HttpResponseTemplates.YoutubePlaylistSnippetResponse;
using DiscordBotCS.HttpResponseTemplates.YoutubePlaylistItemsSnippetResponse;

namespace DiscordBotCS.Commands
{
    public class YoutubePlaylistModule : ModuleBase<SocketCommandContext>
    {
        private static Regex playlistUrlRegex = new Regex(@"(https://)?(www\.)?youtube.com/playlist\?list=([A-Za-z0-9_-]+)");

        [Command("playlist")]
        public async Task YoutubePlaylistCommand(string playlistUrl)
        {
            await Task.Run(async () =>
            {
                string apiKey = DiscordBot.Credentials.YoutubeApiKey;
                var urlMatch = playlistUrlRegex.Match(playlistUrl);
                if (!urlMatch.Success)
                {
                    await ReplyAsync("Provided Url does not appear to be a valid youtube playlist url.");
                }
                var playlistId = urlMatch.Groups[3].Value;

                // Get playlist title:
                string playlistTitle;
                try
                {
                    playlistTitle = await GetPlaylistTitle(apiKey, playlistId);
                }
                catch (Exception ex)
                {
                    return;
                }

                using var typing = Context.Channel.EnterTypingState();
                var queryUrl = @"https://www.googleapis.com/youtube/v3/playlistItems?part=snippet&playlistId=[PLAYLIST_ID]&maxResults=50&key=[API_KEY]";
                queryUrl = queryUrl.Replace("[PLAYLIST_ID]", urlMatch.Groups[3].ToString()).Replace("[API_KEY]", apiKey);
                Console.WriteLine($"Query Url: {queryUrl}");

                List<(string thumbnailUrl, string title, string url, string description)> videos = new List<(string, string, string, string)>();
                bool moreVideos = true;
                while (moreVideos)
                {
                    var response = await DiscordBot.HttpClient.GetAsync(queryUrl);
                    try
                    {
                        response.EnsureSuccessStatusCode();
                    }
                    catch (Exception ex)
                    {
                        await ReplyAsync($"An error occurred: {ex.Message}");
                        return;
                    }
                    var json = await response.Content.ReadAsStringAsync();
                    YoutubePlaylistItemsSnippetResponse playlistItemsData = YoutubePlaylistItemsSnippetResponse.GetPlaylistSnippetResponse(json);

                    foreach (var item in playlistItemsData.Items)
                    {
                        videos.Add((item.Snippet.Thumbnails?.Default?.Url ?? string.Empty, 
                            item.Snippet.Title, 
                            $"https://youtube.com/watch?v={item.Snippet.ResourceId.VideoId}/", 
                            item.Snippet.Description));
                    }

                    if (string.IsNullOrEmpty(playlistItemsData.NextPageToken))
                    {
                        moreVideos = false;
                    }
                    else
                    {
                        Console.WriteLine($"Next Page Token: {playlistItemsData.NextPageToken}");
                        queryUrl = @"https://www.googleapis.com/youtube/v3/playlistItems?part=snippet&playlistId=[PLAYLIST_ID]&maxResults=50&key=[API_KEY]&pageToken=[PAGE_TOKEN]";
                        queryUrl = queryUrl.Replace("[PLAYLIST_ID]", playlistId).Replace("[API_KEY]", apiKey).Replace("[PAGE_TOKEN]", playlistItemsData.NextPageToken);
                    }
                }

                // build html
                string html = File.ReadAllText("PlaylistTemplate.html");
                string tableRows = string.Empty;
                foreach (var video in videos)
                {
                    tableRows += $"<tr>\n" +
                        $"<td><img src='{video.thumbnailUrl}' /></td>" +
                        $"<td><a href={video.url} target='_blank'>{video.title}</a></td>\n" +
                        $"</tr>\n";
                }
                html = html.Replace("[TITLE]", playlistTitle).Replace("[CONTENT]", tableRows);

                using var stream = new MemoryStream();
                var writer = new StreamWriter(stream);
                writer.Write(html);
                writer.Flush();
                stream.Position = 0;

                typing.Dispose();
                await Context.Channel.SendFileAsync(stream: stream, "playlist.html", text: "Here's your playlist :)");
            });
        }

        private async Task<string> GetPlaylistTitle(string apiKey, string playlistId)
        {
            var queryUrl = @"https://www.googleapis.com/youtube/v3/playlists?part=snippet&key=[API_KEY]&id=[PLAYLIST_ID]";
            queryUrl = queryUrl.Replace("[PLAYLIST_ID]", playlistId).Replace("[API_KEY]", apiKey);
            Console.WriteLine($"Query Url: {queryUrl}");
            var response = await DiscordBot.HttpClient.GetAsync(queryUrl);
            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                await ReplyAsync($"An error occurred: {ex.Message}");
                throw new Exception(ex.Message);
            }
            string json = await response.Content.ReadAsStringAsync();
            YoutubePlaylistSnippetResponse playlistData = YoutubePlaylistSnippetResponse.GetPlaylistSnippetResponse(json);
            string playlistTitle = playlistData.Items.First().Snippet.Title;
            return playlistTitle;
        }
    }
}
