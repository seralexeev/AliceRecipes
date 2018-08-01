using Newtonsoft.Json;

namespace AliceKit.Protocol {
  public class ItemsListCard : CardModel {
    public override CardType Type => CardType.ItemsList;

    [JsonProperty("header")]
    public ItemsListCardHeader Header { get; set; }

    [JsonProperty("footer")]
    public ItemsListCardFooter Footer { get; set; }

    [JsonProperty("items")]
    public ItemsListCardItem[] Items { get; set; }
  }
}
