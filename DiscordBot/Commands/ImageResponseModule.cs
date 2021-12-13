using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Discord;
using Discord.Commands;

namespace DiscordBot.Commands
{
    public class ImageResponseModule : ModuleBase<SocketCommandContext>
    {
        private static string[] luigiImageFilenames = Directory.GetFiles("Files/Images/Luigi");
        private static string[] marioImageFilenames = Directory.GetFiles("Files/Images/Mario");

        [Command("luigi")]
        public async Task LuigiImageAsync()
        {
            var randomIndex = DiscordBot.Random.Next(luigiImageFilenames.Length);
            var imageFilename = luigiImageFilenames[randomIndex];

            await Context.Channel.SendFileAsync(filePath: imageFilename, 
                text: "Here's a Luigi :)", 
                messageReference: new MessageReference(Context.Message.Id));
        }

        [Command("mario")]
        public async Task MarioImageAsync()
        {
            var randomIndex = DiscordBot.Random.Next(marioImageFilenames.Length);
            var imageFilename = marioImageFilenames[randomIndex];

            await Context.Channel.SendFileAsync(filePath: imageFilename,
                text: "Here's a Mario :D",
                messageReference: new MessageReference(Context.Message.Id));
        }
    }
}
