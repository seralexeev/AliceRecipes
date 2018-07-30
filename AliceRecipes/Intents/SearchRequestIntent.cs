namespace AliceRecipes.Intents {
  public class SearchRequestIntent : IntentBase {
    public string Query { get; }
    public SearchRequestIntent(string query) => Query = query;
  }
}
