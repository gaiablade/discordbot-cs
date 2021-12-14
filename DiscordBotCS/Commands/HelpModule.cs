using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace DiscordBot_CS.Commands
{
    public class HelpModule : ModuleBase<SocketCommandContext>
    {
        [Command("help")]
        public async Task HelpAsync()
        {
            string helpInformation =
                "List Of Commands\n" +
                "1) !mario\n" +
                "2) !luigi\n" +
                "3) !roll[numDice]d[numSides][+/-][number]\n" +
                "4) !help";

            await ReplyAsync(helpInformation);
        }
    }
}
