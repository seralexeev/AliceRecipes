using Newtonsoft.Json;

namespace AliceKit.Protocol {
  public class ResponseModel {
    public ResponseModel(string text) {
      Text = text;
      Tts = text;
    }

    [JsonProperty("text"), MaxLength(1024)]
    public string Text { get; set; }

    [JsonProperty("tts"), MaxLength(1024)]
    public string Tts { get; set; }

    [JsonProperty("end_session")]
    public bool EndSession { get; set; }

    [JsonProperty("card")]
    public CardModel Card { get; set; }

    ButtonModel[] _buttons;

    [JsonProperty("buttons")]
    public ButtonModel[] Buttons {
      get => _buttons != null && Card != null ? GetAsSuggestions() : _buttons;
      set => _buttons = value;
    }

    ButtonModel[] GetAsSuggestions() {
      foreach (var button in _buttons) {
        button.Hide = true;
      }

      return _buttons;
    }
  }
}
