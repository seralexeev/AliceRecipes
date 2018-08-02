using System.Threading.Tasks;
using AliceRecipes.Models;
using Newtonsoft.Json.Linq;

namespace AliceRecipes.Services {
  public interface IRecipeService {
    Task<QueryResult<RecipePreview>> Find(string q, int first = 5, int offset = 0);
    Task<Recipe> Get(int id);
    Task<JObject> UploadImage(RecipePreview recipe);
  }
}
