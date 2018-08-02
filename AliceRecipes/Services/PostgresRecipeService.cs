using System.Linq;
using System.Threading.Tasks;
using AliceRecipes.Models;
using Dapper;
using Newtonsoft.Json.Linq;
using Npgsql;

namespace AliceRecipes.Services {
  public class PostgresRecipeService : IRecipeService {
    readonly string _conStr;

    public PostgresRecipeService(string conStr) {
      _conStr = conStr;
    }

    public async Task<QueryResult<RecipePreview>> Find(string q, int limit = 10, int offset = 0) {
      using (var conn = new NpgsqlConnection(_conStr)) {
        var result = await conn.QueryAsync<RecipePreview>("SELECT * from search_recipes(@q) LIMIT 5", new {
          q
        });

        return new QueryResult<RecipePreview> {
          Items = result.ToArray(),
          PageInfo = new PageInfoResult {
            HasNextPage = true
          },
          TotalCount = 120
        };
      }
    }

    public async Task<Recipe> Get(int id) {
      using (var conn = new NpgsqlConnection(_conStr)) {
        return await conn.QueryFirstAsync<Recipe>("SELECT * from recipes where id = @id", new {
          id
        });
      }
    }

    public Task<JObject> UploadImage(RecipePreview recipe) => throw new System.NotImplementedException();
  }
}
