using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DiscordBotCS.HttpResponseTemplates.PokeAPIAbilityResponse
{
    public class Language
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }
    }

    public class EffectEntry
    {
        [JsonPropertyName("effect")]
        public string Effect { get; set; }

        [JsonPropertyName("language")]
        public Language Language { get; set; }

        [JsonPropertyName("short_effect")]
        public string ShortEffect { get; set; }
    }

    public class VersionGroup
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }
    }

    public class FlavorTextEntry
    {
        [JsonPropertyName("flavor_text")]
        public string FlavorText { get; set; }

        [JsonPropertyName("language")]
        public Language Language { get; set; }

        [JsonPropertyName("version_group")]
        public VersionGroup VersionGroup { get; set; }
    }

    public class Generation
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }
    }

    public class Name
    {
        [JsonPropertyName("language")]
        public Language Language { get; set; }

        [JsonPropertyName("name")]
        public string LocalName { get; set; }
    }

    public class Pokemon2
    {

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }
    }

    public class Pokemon
    {

        [JsonPropertyName("is_hidden")]
        public bool IsHidden { get; set; }

        [JsonPropertyName("pokemon")]
        public Pokemon Pokemon2 { get; set; }

        [JsonPropertyName("slot")]
        public int Slot { get; set; }
    }

    public class PokeAPIAbilityResponse
    {
        [JsonPropertyName("effect_changes")]
        public List<object> EffectChanges { get; set; }

        [JsonPropertyName("effect_entries")]
        public List<EffectEntry> EffectEntries { get; set; }

        [JsonPropertyName("flavor_text_entries")]
        public List<FlavorTextEntry> FlavorTextEntries { get; set; }

        [JsonPropertyName("generation")]
        public Generation Generation { get; set; }

        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("is_main_series")]
        public bool IsMainSeries { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("names")]
        public List<Name> Names { get; set; }

        [JsonPropertyName("pokemon")]
        public List<Pokemon> Pokemon { get; set; }

        public static PokeAPIAbilityResponse GetAbilityResponse(string json)
        {
            return JsonSerializer.Deserialize<PokeAPIAbilityResponse>(json) ?? new PokeAPIAbilityResponse { };
        }
    }
}
