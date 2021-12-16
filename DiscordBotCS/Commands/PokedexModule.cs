using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Discord;
using Discord.Commands;

using DiscordBotCS.HttpResponseTemplates;

namespace DiscordBotCS.Commands
{
    public class PokedexModule : ModuleBase<SocketCommandContext>
    {
        [Command("dex")]
        public async Task PokedexAsync([Remainder]string name)
        {
            using var typing = Context.Channel.EnterTypingState();

            name = name.Trim().ToLower().Replace(' ', '_');
            string url = $@"https://pokeapi.co/api/v2/pokemon/{name}";
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
                var types = string.Join(", ", pokemonData.Types.Select(type => type.TypeDetails.Name));
                var abilities = string.Join(", ", pokemonData.Abilities.Select(a => a.AbilityDetails.Name));

                // Create embed
                var embed = new EmbedBuilder();
                embed.WithImageUrl(imageUrl);
                embed.AddField(new EmbedFieldBuilder().WithName("Name").WithValue(pokemonName).WithIsInline(true));
                embed.AddField(new EmbedFieldBuilder().WithName("Type(s)").WithValue(types).WithIsInline(true));
                embed.AddField(new EmbedFieldBuilder().WithName("Abilities").WithValue(abilities).WithIsInline(true));

                await ReplyAsync(embed: embed.Build());
            }
            catch (Exception ex)
            {
                await ReplyAsync($"Pokedex Command threw an exception: {ex.Message}");
            }
        }
    }
}
