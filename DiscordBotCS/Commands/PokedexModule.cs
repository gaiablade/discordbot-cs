using Discord;
using Discord.Commands;

using DiscordBotCS.HttpResponseTemplates.PokeAPIAbilityResponse;
using DiscordBotCS.HttpResponseTemplates.PokeAPIPokemonResponse;

namespace DiscordBotCS.Commands
{
    public class PokedexModule : ModuleBase<SocketCommandContext>
    {
        [Command("dex")]
        public async Task PokedexAsync([Remainder]string paramString)
        {
            // Action could take a while, show that the bot is doing work
            using var typing = Context.Channel.EnterTypingState();

            var @params = paramString.Trim().Split(' ');
            string command = "pokedex";
            string name = string.Empty; // pokemon name

            if (@params.Length > 1)
            {
                switch (@params[0])
                {
                    // Pokemon data is the default
                    case "pokemon":
                    case "mon":
                    case "pkmn":
                    case "poke":
                        name = paramString.Trim();
                        break;
                    case "abilities":
                    case "abil":
                    case "ability":
                        command = "abilities";
                        name = String.Join(' ', @params.TakeLast(@params.Length - 1));
                        break;
                    case "moves":
                    case "move":
                        command = "moves";
                        name = String.Join(' ', @params.TakeLast(@params.Length - 1));
                        break;
                    default:
                        name = paramString.Trim();
                        break;
                }
            }
            else
            {
                name = paramString.Trim();
            }

            name = name.Trim().ToLower().Replace(' ', '_');
            string url = string.Empty;

            // If user types "surprise" as the pokemon name, a random pokemon is chosen instead
            if (name == "surprise")
            {
                var randomPokemon = DiscordBot.GetRandomNumber(898) + 1;
                url = $@"https://pokeapi.co/api/v2/pokemon/{randomPokemon}";
            }
            else
            {
                url = $@"https://pokeapi.co/api/v2/pokemon/{name}";
            }

            var response = await DiscordBot.HttpClient.GetAsync(url);

            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException ex)
            {
                await ReplyAsync($"PokeAPI returned a status code of {ex.StatusCode}");
                return;
            }

            try
            {
                var json = await response.Content.ReadAsStringAsync();
                var pokemonData = PokeAPIPokemonResponse.GetPokedexData(json: json ?? "");

                // Extract data we will show in the message:
                var pokemonName = pokemonData.Name;
                var imageUrl = pokemonData.Sprites.FrontDefault;

                // Create embed
                var embed = new EmbedBuilder();
                embed.WithImageUrl(imageUrl);
                switch (command)
                {
                    case "pokedex":
                        var types = string.Join(", ", pokemonData.Types.Select(type => type.TypeDetails.Name));
                        var weight = pokemonData.Weight;
                        var dexNumber = pokemonData.Id;
                        embed.AddField(new EmbedFieldBuilder().WithName("Name").WithValue(pokemonName).WithIsInline(true));
                        embed.AddField(new EmbedFieldBuilder().WithName("Type(s)").WithValue(types).WithIsInline(true));
                        embed.AddField(new EmbedFieldBuilder().WithName("Weight").WithValue(weight).WithIsInline(true));
                        embed.AddField(new EmbedFieldBuilder().WithName("Global Dex #").WithValue(dexNumber).WithIsInline(true));
                        break;
                    case "moves":
                        // TODO GET MOVE DETAILS
                        embed.AddField(new EmbedFieldBuilder().WithName("Name").WithValue(pokemonName).WithIsInline(true));
                        break;
                    case "abilities":
                        // TODO GET ABILITY DETAILS
                        embed.AddField(new EmbedFieldBuilder().WithName("Name").WithValue(pokemonName).WithIsInline(true));
                        //var abilities = string.Join(", ", pokemonData.Abilities.Select(a => a.AbilityDetails.Name));
                        var abilitiyUrls = pokemonData.Abilities.Select(a => a.AbilityDetails.Url);
                        foreach (var URL in abilitiyUrls)
                        {
                            var abilityHttpResponse = await DiscordBot.HttpClient.GetAsync(URL);
                            try
                            {
                                abilityHttpResponse.EnsureSuccessStatusCode();
                            }
                            catch (Exception ex)
                            {
                                return;
                            }
                            var abilityJson = await abilityHttpResponse.Content.ReadAsStringAsync();
                            var abilityData = PokeAPIAbilityResponse.GetAbilityResponse(abilityJson);
                            embed.AddField(new EmbedFieldBuilder()
                                .WithName(abilityData.Name)
                                .WithValue(abilityData.FlavorTextEntries.Where(e => e.Language.Name == "en").Select(e => e.FlavorText).First())
                                .WithIsInline(false)); ;
                        }
                        //embed.AddField(new EmbedFieldBuilder().WithName("Abilities").WithValue(abilities).WithIsInline(true));
                        break;
                }

                await ReplyAsync(embed: embed.Build());
            }
            catch (Exception ex)
            {
                await ReplyAsync($"Pokedex Command threw an exception: {ex.Message}");
            }
        }
    }
}
