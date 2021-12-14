using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

using Discord;
using Discord.API;
using Discord.Commands;
using Discord.Net;
using Discord.WebSocket;

using DiscordBotCS.Debug;

namespace DiscordBotCS
{
    public class DiscordBot
    {
        public static LogManager LogManager { get; private set; } = new LogManager(@"logs\bot\Discord Bot - [DT].txt");
        public static RandomNumberGenerator RandomNumberGenerator { get; private set; } = RandomNumberGenerator.Create();
        public static DiscordSocketClient? Client { get; private set; }
        private CommandService? commandService;

        // Program Entry-Point
        public static void Main(string[] args)
            => new DiscordBot().MainAsync().GetAwaiter().GetResult();

        public async Task MainAsync()
        {
            // Import credentials (bot token) from file
            Credentials credentials = Credentials.GetCredentials("credentials.json");

            // Create bot Client to receive and respond to messages
            Client = new DiscordSocketClient();
            Client.Log += LogManager.LogDiscordMessage; // Assigns function that handles any log messages the bot receives
            Client.MessageReceived += HandleMessageReceivedAsync; // Assigns function that handles messages the bot receives

            // Login and start bot
            await Client.LoginAsync(TokenType.Bot, credentials.token);
            await Client.StartAsync();

            // Create command service, find and load all commands
            commandService = new CommandService();
            await commandService.AddModulesAsync(assembly: Assembly.GetEntryAssembly(), services: null);

            // Set status message of bot
            await Client.SetActivityAsync(new Game("Being rewritten in C#...", ActivityType.Playing, ActivityProperties.None));

            // Delays indefinitely to keep bot running
            await Task.Delay(-1);
        }

        // This function is called every time a message is posted in a channel the bot has access to.
        // Message information is contained in the SocketMessage messageParam parameter
        public async Task HandleMessageReceivedAsync(SocketMessage messageParam)
        {
            // CommandService is initialized in MainAsync, so it should not be null
            if (commandService == null) return;

            // This will be null if the message is a system message (not a user message)
            var message = messageParam as SocketUserMessage;
            if (message == null) return;

            int argPos = 0;


            // Check if message is a command and isn't coming from another bot
            if (!(message.HasCharPrefix('!', argPos: ref argPos)
                || message.HasMentionPrefix(Client?.CurrentUser, ref argPos))
                || message.Author.IsBot)
                return;

            // Handle dice roll command separately, NEEDS A BETTER SOLUTION
            // NOTE: Discord.NET's CommandService does not support commands that match regex patterns, they normally require spaces before parameters
            //  Because of this, the roll command must be handled manually.
            if (message.HasStringPrefix("!roll", ref argPos))
            {
                await DiceRollModule.DiceRoll(message);
                return;
            }

            // Search for corresponding command and execute it
            var context = new SocketCommandContext(Client, message);
            await commandService.ExecuteAsync(context: context, argPos: argPos, services: null);
        }

        public static int GetRandomNumber(int toExclusive)
        {
            return RandomNumberGenerator.GetInt32(toExclusive);
        }
    }
}