using Newtonsoft.Json;

namespace AliceRecipes.Intents {
  public abstract class IntentBase {
    static readonly JsonSerializerSettings SerializerSettings = new JsonSerializerSettings {
      ReferenceLoopHandling = ReferenceLoopHandling.Ignore
    };

    public string IntentName => GetType().Name;

    public override string ToString() =>
      IntentName + ": " + JsonConvert.SerializeObject(this, Formatting.Indented, SerializerSettings);
  }
}
