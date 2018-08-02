using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AliceKit.Helpers;
using AliceKit.Services;
using AliceRecipes.Config;
using AliceRecipes.Models;
using Newtonsoft.Json.Linq;
using static System.IO.File;
using static AliceKit.Helpers.PathEx;

namespace AliceRecipes.Services.GraphQL {
  public class GraphQLRecipeService : IRecipeService {
    readonly GraphQLClient _client;
    readonly IImageService _imageService;
    readonly AliceConfig _config;
    readonly string _getRecipeById;
    readonly string _searchRecipes;
    readonly string _updateAliceImageId;
    readonly PathEx _queries = AppRoot / "Services" / "GraphQL" / "queries";

    public GraphQLRecipeService(GraphQLClient client, IImageService imageService, AliceConfig config) {
      _client = client;
      _config = config;
      _imageService = imageService;

      _getRecipeById = ReadAllText(_queries / "getRecipeById.gql");
      _searchRecipes = ReadAllText(_queries / "searchRecipes.gql");
      _updateAliceImageId = ReadAllText(_queries / "updateAliceImageId.gql");
    }

    public async Task<QueryResult<RecipePreview>> Find(string q, int first = 5, int offset = 0) {
      var result = await Query<QueryResult<RecipePreview>>(_searchRecipes, new {
        q,
        offset,
        first
      });

      UploadImages(result.Items.Where(x => x.AliceImageId == null));

      return result;
    }

    async Task UploadImages(IEnumerable<RecipePreview> recipes) {
      foreach (var recipe in recipes) {
        try {
          await UploadImage(recipe);
        }
        catch (Exception e) {
          Console.WriteLine(e);
        }
      }
    }

    public Task<Recipe> Get(int id) => Query<Recipe>(_getRecipeById, new {id});

    public async Task<JObject> UploadImage(RecipePreview recipe) {
      var aliceImageId = "1030494/9825443721439c9ba843";
      if (recipe.Image != "/images/no-image") {
        var aliceImage = await _imageService.UploadImage(string.Format(_config.ImageUrl, recipe.Image));
        aliceImageId = aliceImage.Image.Id;
      }

      return await _client.Query<JObject>(_updateAliceImageId, new {
        id = recipe.Id,
        aliceImageId
      });
    }

    async Task<T> Query<T>(string query, object variables) {
      var res = await _client.Query<GqlQuery<T>>(query, variables);
      return res.Query;
    }

    class GqlQuery<T> {
      public T Query { get; set; }
    }
  }
}
