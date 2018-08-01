using Newtonsoft.Json;

namespace AliceKit.Protocol {
  public class BigImageCard : CardModel {
    public BigImageCard(string imageId) {
      ImageId = imageId;
    }

    public override CardType Type => CardType.BigImage;

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
