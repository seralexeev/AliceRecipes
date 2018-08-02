using Newtonsoft.Json;

namespace AliceRecipes.Config {
  public class AliceConfig {
    [JsonProperty("oauth")]
    public string OAuth { get; set; }

    [JsonProperty("skill_id")]
    public string SkillId { get; set; }

    [JsonProperty("graphql_url")]
    public string GraphqlUrl { get; set; }

    [JsonProperty("image_url")]
    public string ImageUrl { get; set; }
  }
}
