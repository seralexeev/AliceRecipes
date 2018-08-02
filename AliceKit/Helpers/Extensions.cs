using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AliceKit.Framework;
using static Newtonsoft.Json.JsonConvert;

namespace AliceKit.Helpers {
  public static class Extensions {
    static readonly ThreadLocal<Random> Rnd = new ThreadLocal<Random>(() => new Random());

    public static T GetRandom<T>(this T[] ar) => ar[Rnd.Value.Next(ar.Length)];

    static readonly string[] Terminators = {".", "!", "?", ";", ",", " "};

    public static string TruncateBySentence(this string str, int count = 300) {
      if (str == null || str.Length <= count) {
        return str;
      }

      count -= 3;

      var sub = str.Substring(0, count + 1);
      var index = Terminators
        .Select(x => sub.LastIndexOf(x, StringComparison.Ordinal))
        .Aggregate((max, current) => (max < current ? current : max));

      var result = sub.Substring(0, index);
      return result + (result[result.Length - 1] == '.' ? ".." : "...");
    }


    public static void Deconstruct<TKey, TValue>(
      this KeyValuePair<TKey, TValue> source, out TKey key, out TValue value) {
      key = source.Key;
      value = source.Value;
    }

    public static bool IsInstantIntent(this IntentBase intent) =>
      typeof(IIntentHandler<>).MakeGenericType(intent.GetType()).IsInstanceOfType(intent);

    public static async Task<T> PostAsync<T>(this HttpClient client, string url, object obj,
      Dictionary<string, string> headers = null) {
      var request = new HttpRequestMessage {
        Method = HttpMethod.Post,
        RequestUri = new Uri(url),
        Content = new StringContent(SerializeObject(obj), Encoding.UTF8, "application/json"),
      };

      if (headers != null) {
        foreach (var (key, value) in headers) {
          request.Headers.Add(key, value);
        }
      }

      var result = await client.SendAsync(request);
      var json = await result.Content.ReadAsStringAsync();
      if (!result.IsSuccessStatusCode) {
        throw new HttpRequestException(json);
      }

      return DeserializeObject<T>(json);
    }

    public static IEnumerable<Type> FindBlocks() => Assembly
      .GetEntryAssembly()
      .GetTypes()
      .Where(x => typeof(BlockBase).IsAssignableFrom(x));
  }

  public class PathEx {
    readonly string _path;
    public PathEx(string path) => _path = path;
    public static PathEx AppRoot => new PathEx(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
    public static PathEx operator /(PathEx path, string seg) => new PathEx(Path.Combine(path._path, seg));
    public static implicit operator string(PathEx path) => path._path;
  }
}
