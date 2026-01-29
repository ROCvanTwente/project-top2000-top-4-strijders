using System.Text.Json.Serialization;

public class YouTubeSearchResponse
{
    [JsonPropertyName("items")]
    public List<Item> Items { get; set; }

    public class Item
    {
        [JsonPropertyName("id")]
        public IdData Id { get; set; }
    }

    public class IdData
    {
        [JsonPropertyName("videoId")]
        public string VideoId { get; set; }
    }
}
