using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static AliceRecipes.Constants.FixedList;

namespace AliceRecipes {
  public class SessionModel {
    [JsonProperty("new")] public bool New { get; set; }
    [JsonProperty("session_id")] public string SessionId { get; set; }
    [JsonProperty("message_id")] public int MessageId { get; set; }
    [JsonProperty("skill_id")] public string SkillId { get; set; }
    [JsonProperty("user_id")] public string UserId { get; set; }
  }

  public class ResponseModel {
    private ButtonModel[] _buttons;

    public ResponseModel(string text) {
      Text = text;
      Tts = text;
    }

    [JsonProperty("text")] public string Text { get; set; }
    [JsonProperty("tts")] public string Tts { get; set; }
    [JsonProperty("end_session")] public bool EndSession { get; set; }

    [JsonProperty("buttons")]
    public ButtonModel[] Buttons {
      get => _buttons != null && Card != null ? GetAsSuggestions() : _buttons;
      set => _buttons = value;
    }

    private ButtonModel[] GetAsSuggestions() {
      foreach (var button in _buttons) {
        button.Hide = true;
      }

      return _buttons;
    }

    [JsonProperty("card")] public CardModel Card { get; set; }
  }

  public class ButtonModel {
    public ButtonModel(string title, bool hide = true, string url = null) {
      Title = title;
      Hide = hide;
      Url = url;
      Payload = new ButtonPayload {
        Text = title
      };
    }

    [JsonProperty("title")] public string Title { get; set; }
    [JsonProperty("url")] public string Url { get; set; }
    [JsonProperty("hide")] public bool Hide { get; set; }
    [JsonProperty("payload")] public object Payload { get; }
  }

  public class ButtonPayload {
    public string Text { get; set; }
  }

  public class MetaModel {
    [JsonProperty("locale")] public string Locale { get; set; }
    [JsonProperty("timezone")] public string Timezone { get; set; }
    [JsonProperty("client_id")] public string ClientId { get; set; }
  }

  public class AliceRequest {
    [JsonProperty("meta")] public MetaModel Meta { get; set; }
    [JsonProperty("request")] public RequestModel Request { get; set; }
    [JsonProperty("session")] public SessionModel Session { get; set; }
    [JsonProperty("version")] public string Version { get; set; }
  }

  public enum RequestType {
    SimpleUtterance,
    ButtonPressed
  }

  public class RequestModel {
    [JsonProperty("command")] public string Command { get; set; }
    [JsonProperty("type")] public RequestType Type { get; set; }
    [JsonProperty("original_utterance")] public string OriginalUtterance { get; set; }
    [JsonProperty("payload")] public ButtonPayload Payload { get; set; }

    public string GetSatanized(params string[] exclude) {
      var words = new HashSet<string>(GarbageWords.Concat(exclude));
      return string.Join(" ",
        (OriginalUtterance ?? Command ?? Payload?.Text ?? "").Split().Where(x => !words.Contains(x)));
    }

    public bool Contains(string val) => GetOriginal().Contains(val, StringComparison.InvariantCultureIgnoreCase);

    public string GetOriginal() => (OriginalUtterance ?? Command ?? Payload?.Text ?? "").ToLowerInvariant();
  }

  public class AliceResponse {
    [JsonProperty("response")] public ResponseModel Response { get; set; }
    [JsonProperty("session")] public SessionModel Session { get; set; }
    [JsonProperty("version")] public string Version { get; set; } = "1.0";
  }

  public enum CardType {
    BigImage,
    ItemsList
  }

  public abstract class CardModel {
    public abstract CardType Type { get; }
  }

  public class BigImageCard : CardModel {
    public BigImageCard(string imageId) {
      ImageId = imageId;
    }

    public override CardType Type => CardType.BigImage;
    [JsonProperty("image_id")] public string ImageId { get; set; }
    [JsonProperty("title")] public string Title { get; set; }
    [JsonProperty("description")] public string Description { get; set; }
    [JsonProperty("button")] public CardButton Button { get; set; }
  }

  public class ItemsListCard : CardModel {
    public override CardType Type => CardType.ItemsList;
    [JsonProperty("header")] public ItemsListCardHeader Header { get; set; }
    [JsonProperty("footer")] public ItemsListCardFooter Footer { get; set; }
    [JsonProperty("items")] public ItemsListCardItem[] Items { get; set; }
  }

  public class ItemsListCardItem {
    [JsonProperty("image_id")] public string ImageId { get; set; }
    [JsonProperty("title")] public string Title { get; set; }
    [JsonProperty("description")] public string Description { get; set; }
    [JsonProperty("button")] public CardButton Button { get; set; }
  }

  public class ItemsListCardFooter {
    public ItemsListCardFooter(string text) => Text = text;
    [JsonProperty("text")] public string Text { get; set; }
    [JsonProperty("button")] public CardButton Button { get; set; }
  }

  public class ItemsListCardHeader {
    public ItemsListCardHeader(string text) => Text = text;
    [JsonProperty("text")] public string Text { get; set; }
  }

  public class CardButton {
    public CardButton(string text, string url = null) {
      Text = text;
      Url = url;
      Payload = new ButtonPayload {
        Text = text
      };
    }

    [JsonProperty("text")] public string Text { get; set; }
    [JsonProperty("url")] public string Url { get; set; }
    [JsonProperty("payload")] public ButtonPayload Payload { get; }
  }
}
