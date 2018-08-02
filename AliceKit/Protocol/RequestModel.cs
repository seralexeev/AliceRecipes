using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;
using AliceKit.Helpers;

namespace AliceKit.Protocol {
  public class RequestModel {
    [JsonProperty("command")]
    public string Command { get; set; }

    [JsonProperty("type")]
    public RequestType Type { get; set; }

    [JsonProperty("original_utterance")]
    public string OriginalUtterance { get; set; }

    [JsonProperty("payload")]
    public ButtonPayload Payload { get; set; }

    public string GetSanitized(params string[] exclude) {
      var words = new HashSet<string>(FixedList.GarbageWords.Concat(exclude),
        StringComparer.InvariantCultureIgnoreCase);

      var split = (OriginalUtterance ?? Command ?? Payload?.Text ?? "")
        .Split(new[] {' ', ',', '.', '!', '?', ':', ';'}, StringSplitOptions.RemoveEmptyEntries);

      return string.Join(" ", split.Where(x => /* !StopWords.Russian.Contains(x) && */ !words.Contains(x)));
    }

    public bool Contains(params string[] val) {
      var text = GetOriginal().ToLowerInvariant();
      return val.Any(x => text.Contains(x));
    }

    public string GetOriginal() => (OriginalUtterance ?? Command ?? Payload?.Text ?? "").ToLowerInvariant();
  }
}
