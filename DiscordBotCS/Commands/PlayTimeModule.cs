using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Discord;
using Discord.Commands;

using DiscordBotCS.Commands;
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
                    // Get top 5 played games:
                    List<(string name, int minutesPlayed)> games = await PlayTimeRecorder.GetPlayTimeWrappedAsync(Context.User.Id, limit: 5);
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
                             Value = $"Hours: {hoursPlayed}",
                             IsInline = true,
                        });
                    }
              
                    
                    await ReplyAsync(embed: embed.Build());
                    break;
                default:
                    await ReplyAsync("Unrecognized playtime command!");
                    break;
            }
        }
    }
}
