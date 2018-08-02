namespace AliceRecipes.Models {
  public class Recipe : RecipePreview {
    public Nodes<Step> Steps { get; set; }
    public Nodes<IngredientNode> Ingredients { get; set; }
    public int PortionsCount { get; set; }
  }
}
