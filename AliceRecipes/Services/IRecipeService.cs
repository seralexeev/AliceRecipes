using System;
using System.Linq;
using System.Threading.Tasks;
using AliceRecipes.Models;
using Dapper;
using Npgsql;

namespace AliceRecipes.Services {
  public interface IRecipeService {
    Task<QueryResult<RecipePreview>> Find(string q, int limit = 10, int offset = 0);
    Task<Recipe> Get(int id);
  }

  public class PostgresRecipeService : IRecipeService {
    private string connString = Environment.GetEnvironmentVariable("CON_STR");

    public async Task<QueryResult<RecipePreview>> Find(string q, int limit = 10, int offset = 0) {
      using (var conn = new NpgsqlConnection(connString)) {
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
      using (var conn = new NpgsqlConnection(connString)) {
        return await conn.QueryFirstAsync<Recipe>("SELECT * from recipes where id = @id", new {
          id
        });
      }
    }
  }

  public class RecipeService : IRecipeService {
    private readonly GraphQL _gql;

    public RecipeService(GraphQL gql) {
      _gql = gql;
    }

    public async Task<QueryResult<RecipePreview>> Find(string q, int limit = 10, int offset = 0) {
      var fields = "id name image";
      var queryName = $"searchRecipes(offset: {offset}, first: {limit}, _search: \"{q}\")";

      var res = await _gql.Query<SearchResult<RecipePreview>>(queryName, fields);

      return res.Query;
    }


    public Task<Recipe> Get(int id) {
      return null;
    }
  }

  public class SearchResult<T> {
    public QueryResult<T> Query { get; set; }
  }

  public class QueryResult<T> {
    public int TotalCount { get; set; }
    public PageInfoResult PageInfo { get; set; }
    public T[] Items { get; set; }
  }

  public class PageInfoResult {
    public bool HasNextPage { get; set; }
  }
}
