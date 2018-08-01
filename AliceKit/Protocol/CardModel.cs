using Newtonsoft.Json;

namespace AliceKit.Protocol {
  public abstract class CardModel {
    [JsonProperty("type")]
    public abstract CardType Type { get; }
  }
}
