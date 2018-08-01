using Newtonsoft.Json;

namespace AliceKit.Protocol {
  public class CardButton {
    public CardButton(string text, string url = null) {
      Text = text;
      Url = url;
      Payload = new ButtonPayload {
        Text = text
      };
    }

    [JsonProperty("text"), MaxLength(128)]
    public string Text { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("payload")]
    public ButtonPayload Payload { get; }
  }
}
