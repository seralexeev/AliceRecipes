using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AliceKit.Helpers;
using static Newtonsoft.Json.JsonConvert;

namespace AliceRecipes.Services.GraphQL {
  public class GraphQLClient {
    readonly string _url;
    static readonly HttpClient HttpClient = new HttpClient();

    public GraphQLClient(string url) => _url = url ?? throw new ArgumentNullException(nameof(url));

    public async Task<T> Query<T>(string query, object variables) => (await HttpClient.PostAsync<Result<T>>(_url, new {
      query,
      variables
    })).Data;

    class Result<T> {
      public T Data { get; set; }
    }
  }
}
