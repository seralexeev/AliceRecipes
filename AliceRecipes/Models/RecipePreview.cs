namespace AliceRecipes.Models {
  public class RecipePreview {
    public int Id { get; set; }
    public string Name { get; set; }
    public string Image { get; set; }
    public string AliceImageId { get; set; }
    public string Description { get; set; }
    public string Note { get; set; }
  }

//  public class ProductMap : EntityMap<RecipePreview> {
//    public ProductMap() {
//      Map(p => p.AliceImageId).ToColumn("alice_image_id");
//    }
//  }

  public class Recipe {
    public int Id { get; set; }
    public string Name { get; set; }
    public string Image { get; set; }
    public string AliceImageId { get; set; }
    public string Description { get; set; }
    public string Note { get; set; }
    public Ingredient[] Ingredients { get; set; }
  }

  public class Ingredient {
    public int Id { get; set; }
    public string Name { get; set; }
  }
}
