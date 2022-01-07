using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DiscordBotCS.HttpResponseTemplates.YoutubePlaylistItemsSnippetResponse
{
    public class Default
    {
        [JsonPropertyName("url")]
        public string Url { get; set; } = string.Empty;

        [JsonPropertyName("width")]
        public int Width { get; set; }

        [JsonPropertyName("height")]
        public int Height { get; set; }
    }

    public class Medium
    {
        [JsonPropertyName("url")]
        public string Url { get; set; } = string.Empty;

        [JsonPropertyName("width")]
        public int Width { get; set; }

        [JsonPropertyName("height")]
        public int Height { get; set; }
    }

    public class High
    {
        [JsonPropertyName("url")]
        public string Url { get; set; } = string.Empty;

        [JsonPropertyName("width")]
        public int Width { get; set; }

        [JsonPropertyName("height")]
        public int Height { get; set; }
    }

    public class Standard
    {
        [JsonPropertyName("url")]
        public string Url { get; set; } = string.Empty;

        [JsonPropertyName("width")]
        public int Width { get; set; }

        [JsonPropertyName("height")]
        public int Height { get; set; }
    }

    public class Maxres
    {
        [JsonPropertyName("url")]
        public string Url { get; set; } = string.Empty;

        [JsonPropertyName("width")]
        public int Width { get; set; }

        [JsonPropertyName("height")]
        public int Height { get; set; }
    }

    public class Thumbnails
    {
        [JsonPropertyName("default")]
        public Default Default { get; set; }

        [JsonPropertyName("medium")]
        public Medium Medium { get; set; }

        [JsonPropertyName("high")]
        public High High { get; set; }

        [JsonPropertyName("standard")]
        public Standard Standard { get; set; }

        [JsonPropertyName("maxres")]
        public Maxres Maxres { get; set; }
    }

    public class ResourceId
    {
        [JsonPropertyName("kind")]
        public string Kind { get; set; }

        [JsonPropertyName("videoId")]
        public string VideoId { get; set; }
    }

    public class Snippet
    {
        [JsonPropertyName("publishedAt")]
        public DateTime PublishedAt { get; set; }

        [JsonPropertyName("channelId")]
        public string ChannelId { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("thumbnails")]
        public Thumbnails Thumbnails { get; set; }

        [JsonPropertyName("channelTitle")]
        public string ChannelTitle { get; set; }

        [JsonPropertyName("playlistId")]
        public string PlaylistId { get; set; }

        [JsonPropertyName("position")]
        public int Position { get; set; }

        [JsonPropertyName("resourceId")]
        public ResourceId ResourceId { get; set; }

        [JsonPropertyName("videoOwnerChannelTitle")]
        public string VideoOwnerChannelTitle { get; set; }

        [JsonPropertyName("videoOwnerChannelId")]
        public string VideoOwnerChannelId { get; set; }
    }

    public class Item
    {
        [JsonPropertyName("kind")]
        public string Kind { get; set; }

        [JsonPropertyName("etag")]
        public string Etag { get; set; }

        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("snippet")]
        public Snippet Snippet { get; set; }
    }

    public class PageInfo
    {
        [JsonPropertyName("totalResults")]
        public int TotalResults { get; set; }

        [JsonPropertyName("resultsPerPage")]
        public int ResultsPerPage { get; set; }
    }

    public class YoutubePlaylistItemsSnippetResponse
    {
        [JsonPropertyName("kind")]
        public string Kind { get; set; }

        [JsonPropertyName("etag")]
        public string Etag { get; set; }

        [JsonPropertyName("nextPageToken")]
        public string NextPageToken { get; set; }

        [JsonPropertyName("items")]
        public List<Item> Items { get; set; } = new List<Item>();

        [JsonPropertyName("pageInfo")]
        public PageInfo PageInfo { get; set; }

        public static YoutubePlaylistItemsSnippetResponse GetPlaylistSnippetResponse(string json)
        {
            return JsonSerializer.Deserialize<YoutubePlaylistItemsSnippetResponse>(json) ?? new YoutubePlaylistItemsSnippetResponse { };
        }
    }

}
