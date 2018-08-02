using System;
using Newtonsoft.Json;

namespace AliceKit.Helpers {
  public class StringConvertor : JsonConverter<string> {
    readonly int _maxLength;

    public StringConvertor(int maxLength) => _maxLength = maxLength;

    public override void WriteJson(JsonWriter writer, string value, JsonSerializer serializer) =>
      writer.WriteValue(value.TruncateBySentence(_maxLength));

    public override string ReadJson(JsonReader reader, Type objectType, string existingValue, bool hasExistingValue,
      JsonSerializer serializer) => throw new NotImplementedException();
  }
}
