using System.Reflection;
using System.Security.Cryptography;

using Discord;
using Discord.Commands;
using Discord.WebSocket;

using DiscordBotCS.DatabaseInteraction;
using DiscordBotCS.Debug;

namespace DiscordBotCS
{
    public class DiscordBot
    {
        // Static objects belong to the class instead of an instance of the class
        public static LogManager LogManager { get; private set; } = new LogManager(@"logs\bot\Discord Bot - [DT].txt");
        public static RandomNumberGenerator RandomNumberGenerator { get; private set; } = RandomNumberGenerator.Create();
        public static DiscordSocketClient? Client { get; private set; }
        public static Credentials Credentials { get; private set; } = Credentials.GetCredentials("credentials.json");
        public static HttpClient HttpClient { get; private set; } = new HttpClient();
        private CommandService? commandService;

        // Program Entry-Point
        public static void Main(string[] args)
            => new DiscordBot().MainAsync().GetAwaiter().GetResult();

        public async Task MainAsync()
        {
            // Create bot Client to receive and respond to messages
            Client = new DiscordSocketClient();
            Client.Log += LogManager.LogDiscordMessage; // Assigns function that handles any log messages the bot receives
            Client.MessageReceived += HandleMessageReceivedAsync; // Assigns function that handles messages the bot receives
            Client.GuildMemberUpdated += HandleGuildMemberUpdatedAsync;

            // Login and start bot
            await Client.LoginAsync(TokenType.Bot, Credentials.token);
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
                || message.HasCharPrefix('_', argPos: ref argPos)
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

        public async Task HandleGuildMemberUpdatedAsync(SocketGuildUser before, SocketGuildUser after)
        {
            if (before.IsBot) return;
            var userId = after.Id;
            var beforeGame = before.Activities.Where(a => a.Type == ActivityType.Playing).FirstOrDefault() ?? null;
            var afterGame = after.Activities.Where(a => a.Type == ActivityType.Playing).FirstOrDefault() ?? null;
            await PlayTimeRecorder.UpdatePlayTime(userId, after.Username, beforeGame?.ToString() ?? null, afterGame?.ToString() ?? null);
            return;
        }

        public static int GetRandomNumber(int toExclusive)
        {
            return RandomNumberGenerator.GetInt32(toExclusive);
        }
    }
}