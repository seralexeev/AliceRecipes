using Newtonsoft.Json;
using static Newtonsoft.Json.JsonConvert;

namespace AliceKit.Framework {
  public abstract class IntentBase {
    static readonly JsonSerializerSettings SerializerSettings = new JsonSerializerSettings {
      ReferenceLoopHandling = ReferenceLoopHandling.Ignore
    };

    public string IntentName => GetType().Name;

    public override string ToString() =>
      IntentName + ": " + SerializeObject(this, Formatting.Indented, SerializerSettings);
  }
}
