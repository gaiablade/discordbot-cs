using System.Text.RegularExpressions;

using Discord;
using Discord.WebSocket;

namespace DiscordBotCS
{
    public class DiceRollModule
    {
        // A regular expression (regex) is a pattern you can apply to a string to determine if it
        // matches. We put parenthesis around parts of the pattern that we want to extract if the
        // match is valid. These parts are the number of dice, the number of sides on the dice, and
        // the optional expression to add or subtract from the result.
        // Key:
        // \d   = represents a digit from 0-9
        // *    = matches any number of the previous character or group (including 0 of them)
        // +    = matches 1 or more of the previous character or group
        // ?    = matches  0 or 1 of the previous character or  group (optional)
        // [+-] = matches either + or - but not both
        private static Regex diceRollCommandRegex = new Regex(@"!roll(\d*)d(\d+)([+-]\d+)?");

        // Snarky responses the bot will respond with if unreasonable requests are made.
        private static string[] diceTypeExceededResponseMessages = new string[]
        {
            "Why are you rolling a d{diceType}? Can you even comprehend what that would look like?",
            "I'll pass...",
            "I'm gonna need you to show me what a d{diceType} would even look like before I try to do that.",
            "Yeah, not my problem.",
            "I think you should reevaluate what got you into a situation involving a d{diceType}."
        };
        private static string[] numberOfDiceExceededResponseMessages = new string[]
        {
            "Really? {numDice} dice?? Are you rolling for the number of atoms in the universe?",
            "It would take a while to roll {numDice} dice, and I have brunch in an hour. Sorry.",
            "Sorry, I gotta go... Umm... Get my cat dressed for school. Good luck with that.",
            "I would happily do this for you if I didn't have a life."
        };

        public static async Task DiceRoll(SocketUserMessage message)
        {
            // Remove any spaces from command
            var content = message.Content.Replace(" ", "");

            // Check if message content matches the pattern and get the parameter information
            var matchInfo = diceRollCommandRegex.Match(content);
            if (!matchInfo.Success)
            {
                await message.ReplyAsync("There was a syntax error in your command, please try again. Example: !roll1d20");
                return;
            }

            // Extract important iniformation from match
            var numDiceStr  = matchInfo.Groups[1].Value;
            var diceTypeStr = matchInfo.Groups[2].Value;
            var additionalExpressionStr = matchInfo.Groups[3].Value;

            // Parse number of dice out of matched string. This parameter is optional, so it defaults to 1
            var numDice = numDiceStr == string.Empty ? 1 : int.Parse(numDiceStr);
            // Parse the number of sides on the dice being rolled
            var diceType = int.Parse(diceTypeStr);

            if (diceType > 5000)
            {
                await message.ReplyAsync(GetDiceTypeExceededResponseMessage(diceType));
                return;
            }
            else if (numDice > 5000)
            {
                await message.ReplyAsync(GetNumberOfDiceExceededResponseMessage(numDice));
                return;
            }

            // Create message embed for a more expressive message
            var embedBuilder = new EmbedBuilder()
                .WithAuthor(new EmbedAuthorBuilder()
                                .WithName(DiscordBot.Client?.CurrentUser.Username ?? "roll-bot")
                                .WithIconUrl(DiscordBot.Client?.CurrentUser?.GetAvatarUrl() ?? string.Empty))
                .AddField(new EmbedFieldBuilder().WithName("Request").WithValue(content).WithIsInline(true));

            // The aggregate function iterates over an array and uses an accumulator to calculate some sort of "sum"
            // We create an initial acccumulator of 0, and on each iteration we add a random number to it.
            // We could also just use a for loop, but this reduces it to one line of code and I like that :)
            int sum = new byte[numDice].Aggregate(0, (sum, next) => sum + DiscordBot.GetRandomNumber(diceType) + 1);

            int total = sum;
            // If the optional expression is present, we add/subtract that from the sum and add a field to the embed
            // to show how we calculate the result
            if (additionalExpressionStr != String.Empty)
            {
                // additionalExpressionStr is something like "+5"
                char sign = additionalExpressionStr[0];
                string number = additionalExpressionStr.Substring(1);

                if (sign == '+')
                    total += int.Parse(number);
                else if (sign == '-')
                    total -= int.Parse(number);

                embedBuilder = embedBuilder.WithTitle($"{total}");
                embedBuilder = embedBuilder.AddField(new EmbedFieldBuilder().WithName("Result").WithValue($"{total}").WithIsInline(true));
                embedBuilder = embedBuilder.AddField(new EmbedFieldBuilder().WithName("Evaluation").WithValue($"[{sum} {sign} {number}] = {total}").WithIsInline(true));
            }
            else
            {
                embedBuilder = embedBuilder.WithTitle($"{total}");
                embedBuilder = embedBuilder.AddField(new EmbedFieldBuilder().WithName("Result").WithValue($"{total}").WithIsInline(true));
            }

            await message.ReplyAsync(embed: embedBuilder.Build());
        }

        public static string GetDiceTypeExceededResponseMessage(int diceType)
        {
            var index = DiscordBot.GetRandomNumber(diceTypeExceededResponseMessages.Length);
            return diceTypeExceededResponseMessages[index].Replace("{diceType}", diceType.ToString());
        }

        public static string GetNumberOfDiceExceededResponseMessage(int numDice)
        {
            var index = DiscordBot.GetRandomNumber(numberOfDiceExceededResponseMessages.Length);
            return numberOfDiceExceededResponseMessages[index].Replace("{numDice}", numDice.ToString());
        }
    }
}
