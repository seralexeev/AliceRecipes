using Newtonsoft.Json;

namespace AliceKit.Protocol {
  public class ItemsListCardHeader {
    public ItemsListCardHeader(string text) => Text = text;

    [JsonProperty("text"), MaxLength(128)]
    public string Text { get; set; }
  }
}
