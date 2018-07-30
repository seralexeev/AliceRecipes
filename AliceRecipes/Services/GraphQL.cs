using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RestSharp;

namespace AliceRecipes.Services {
  public class GraphQL {
    static RestClient _client = new RestClient("...");
//    private readonly HttpClient _httpClient;

    public GraphQL(HttpClient httpClient) {
//      _httpClient = httpClient;
      AppContext.SetSwitch("System.Net.Http.UseSocketsHttpHandler", false);
    }

    public async Task<T> Query<T>(string queryName, string fields) {
      var query = $@"
query {{ query: {queryName} {{
      totalCount
      pageInfo {{
        hasNextPage
      }}
      items: nodes {{
        {fields}
      }}
    }}
  }}
";

//
//      var request = new RestRequest("/graphql", Method.POST);
//
//      request.AddJsonBody(new {
//        query
//      });
//
//      var sw = Stopwatch.StartNew();
//
//      var tcs = new TaskCompletionSource<GraphQLResult<T>>();
//
//      var res = _client.ExecuteAsync<GraphQLResult<T>>(request, response => {
//        tcs.SetResult(JsonConvert.DeserializeObject<GraphQLResult<T>>(response.Content));
//      });
//
//      var result = await tcs.Task;
//
//      Console.WriteLine($"POST: {sw.Elapsed}");
//
//
//      return result.Data;

      HttpClient _httpClient = new HttpClient();


      var sw = Stopwatch.StartNew();
      Console.WriteLine($"HERE");
      var result = await _httpClient.PostAsync("http://localhost:3000/graphql", Json(new {
        query
      }));

      Console.WriteLine($"POST: {sw.Elapsed}");

      var json = await result.Content.ReadAsStringAsync();

      Console.WriteLine($"READ: {sw.Elapsed}");

      var data = JsonConvert.DeserializeObject<GraphQLResult<T>>(json).Data;

      Console.WriteLine($"DES: {sw.Elapsed}");
      return data;
    }

    StringContent Json(object obj) =>
      new StringContent(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json");

    class GraphQLResult<T> {
      public T Data { get; set; }
    }
  }
}
