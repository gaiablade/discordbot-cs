using Discord;
using Discord.Commands;

using DiscordBotCS.DatabaseInteraction;

namespace DiscordBotCS.Commands
{
    public class PlayTimeModule : ModuleBase<SocketCommandContext>
    {
        [Command("playtime")]
        public async Task PlayTimeAsync([Remainder]string commandString)
        {
            if (string.IsNullOrEmpty(commandString))
            {
                await ReplyAsync("Command requires parameters. Try \"playtime wrapped\"");
                return;
            }

            var parameters = commandString.Trim().Split(' ');
            string action = parameters.First();

            switch (action)
            {
                case "wrapped":
                    {
                        var limit = 5;
                        if (parameters.Length > 1)
                        {
                            var parsed = int.TryParse(parameters[1], out limit);
                            if (parsed) limit = Math.Min(limit, 20);
                            else limit = 5;
                        }

                        // Get top X played games:
                        List<(string name, int minutesPlayed)> games = await PlayTimeRecorder.GetPlayTimeWrappedAsync(Context.User.Id, Context.Guild.Id, limit);
                        if (games.Count == 0)
                        {
                            await ReplyAsync("My database doesn't contain any information of your play history!");
                            break;
                        }

                        // Create embed
                        var embed = new EmbedBuilder()
                        {
                            Title = $"{Context.User.Username}'s Most-Played Games"
                        };
                        for (int i = 0; i < games.Count; i++)
                        {
                            double hoursPlayed = Math.Round((double)(games[i].minutesPlayed) / 60.0, 2);
                            embed = embed.AddField(new EmbedFieldBuilder()
                            {
                                Name = $"#{i + 1}) {games[i].name}",
                                Value = $"Hours: {hoursPlayed}"
                            });
                        }
                        await ReplyAsync(embed: embed.Build());
                        break;
                    }
                case "popular":
                    {
                        var limit = 5;
                        if (parameters.Length > 1)
                        {
                            var parsed = int.TryParse(parameters[1], out limit);
                            if (parsed) limit = Math.Min(limit, 20);
                            else limit = 5;
                        }

                        // Get top X popular games
                        var popularGames = await PlayTimeRecorder.GetPopularGames(Context.Guild.Id, limit);
                        if (popularGames.Count == 0)
                        {
                            await ReplyAsync("No stats recorded yet!");
                            break;
                        }

                        var popularGamesEmbed = new EmbedBuilder()
                        {
                            Title = "Most Popular Games this Year!"
                        };
                        for (int i = 0; i < popularGames.Count; i++)
                        {
                            double totalHours = Math.Round((double)(popularGames[i].minutes) / 60.0, 2);
                            popularGamesEmbed.AddField(new EmbedFieldBuilder()
                            {
                                Name = $"#{i + 1}) {popularGames[i].game}",
                                Value = $"Total Hours: {totalHours}, Players: {popularGames[i].numPlayers}"
                            });
                        }

                        await ReplyAsync(embed: popularGamesEmbed.Build());
                        break;
                    }
                default:
                    await ReplyAsync("Unrecognized playtime command!");
                    break;
            }
        }
    }
}
