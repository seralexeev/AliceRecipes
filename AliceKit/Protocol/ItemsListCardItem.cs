using Newtonsoft.Json;

namespace AliceKit.Protocol {
  public class ItemsListCardItem {
    [JsonProperty("image_id")]
    public string ImageId { get; set; }

    [JsonProperty("title"), MaxLength(128)]
    public string Title { get; set; }

    [JsonProperty("description"), MaxLength(256)]
    public string Description { get; set; }

    [JsonProperty("button")]
    public CardButton Button { get; set; }
  }
}
