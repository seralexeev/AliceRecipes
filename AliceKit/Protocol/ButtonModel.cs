using Newtonsoft.Json;

namespace AliceKit.Protocol {
  public class ButtonModel {
    public ButtonModel(string title, bool hide = true, string url = null) {
      Title = title;
      Hide = hide;
      Url = url;
      Payload = new ButtonPayload {
        Text = title
      };
    }

    [JsonProperty("title"), MaxLength(128)]
    public string Title { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("hide")]
    public bool Hide { get; set; }

    [JsonProperty("payload")]
    public ButtonPayload Payload { get; }
  }
}
