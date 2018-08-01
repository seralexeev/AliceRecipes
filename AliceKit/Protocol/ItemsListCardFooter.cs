using Newtonsoft.Json;

namespace AliceKit.Protocol {
  public class ItemsListCardFooter {
    public ItemsListCardFooter(string text) => Text = text;

    [JsonProperty("text"), MaxLength(128)]
    public string Text { get; set; }

    [JsonProperty("button")]
    public CardButton Button { get; set; }
  }
}
