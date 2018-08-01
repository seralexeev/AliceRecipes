using Newtonsoft.Json;

namespace AliceKit.Protocol {
  public class ButtonPayload {
    [JsonProperty("text")]
    public string Text { get; set; }
  }
}
