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
        private static string[] luigiImageFilenames = GetLuigiImagesFilenames();
        private static string[] GetLuigiImagesFilenames()
        {
            if (!Directory.Exists("Files/Images/Luigi"))
            {
                throw new FileNotFoundException("Directory does not exist");
            }
            return Directory.GetFiles("Files/Images/Luigi");
        }

        [Command("luigi")]
        public async Task LuigiImageAsync()
        {
            var randomIndex = DiscordBot.Random.Next(luigiImageFilenames.Length);
            var imageFilename = luigiImageFilenames[randomIndex];

            await Context.Channel.SendFileAsync(filePath: imageFilename, 
                text: "Here's a Luigi :)", 
                messageReference: new MessageReference(Context.Message.Id));
        }
    }
}
