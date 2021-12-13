using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Discord;
using Discord.API;
using Discord.Commands;
using Discord.Net;
using Discord.WebSocket;

using DiscordBot.Debug;

namespace DiscordBot
{
    public class DiscordBot
    {
        public static LogManager LogManager { get; private set; } = new LogManager(@"logs\bot\Discord Bot - [DT].txt");
        public static Random Random { get; private set; } = new Random();
        private DiscordSocketClient? client;
        private CommandService? commandService;

        // Program Entry-Point
        public static void Main(string[] args)
            => new DiscordBot().MainAsync().GetAwaiter().GetResult();

        public async Task MainAsync()
        {
            // Import credentials (bot token) from file
            Credentials credentials = Credentials.GetCredentials("credentials.json");

            // Create bot client to receive and respond to messages
            client = new DiscordSocketClient();
            client.Log += LogManager.LogDiscordMessage; // Assigns function that handles any log messages the bot receives
            client.MessageReceived += HandleMessageReceivedAsync; // Assigns function that handles messages the bot receives

            // Login and start bot
            await client.LoginAsync(TokenType.Bot, credentials.token);
            await client.StartAsync();

            // Create command service, find and load all commands
            commandService = new CommandService();
            await commandService.AddModulesAsync(assembly: Assembly.GetEntryAssembly(), services: null);

            // Set status message of bot
            await client.SetActivityAsync(new Game("Being rewritten in C#...", ActivityType.Playing, ActivityProperties.None));

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

            // Check if message is a command and isn't coming from another bot
            int argPos = 0;
            if (!(message.HasCharPrefix('!', ref argPos)
                || message.HasMentionPrefix(client?.CurrentUser, ref argPos))
                || message.Author.IsBot)
                return;

            // Search for corresponding command and execute it
            var context = new SocketCommandContext(client, message);
            await commandService.ExecuteAsync(context: context, argPos: argPos, services: null);
        }
    }
}