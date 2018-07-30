namespace AliceRecipes.Intents {
  public class RecipeNameSelectionIntent : IntentBase {
    public string Name { get; }
    public RecipeNameSelectionIntent(string name) => Name = name;
  }
}
