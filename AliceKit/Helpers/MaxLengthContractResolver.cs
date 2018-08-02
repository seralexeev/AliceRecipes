using System.Reflection;
using AliceKit.Protocol;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace AliceKit.Helpers {
  public class MaxLengthContractResolver : CamelCasePropertyNamesContractResolver {
    protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization) {
      var property = base.CreateProperty(member, memberSerialization);
      var attr = member.GetCustomAttribute<MaxLengthAttribute>();
      if (attr != null) {
        property.Converter = new StringConvertor(attr.MaxLength);
      }

      return property;
    }
  }
}
