using System;
using System.Linq;
using System.Threading;

namespace AliceRecipes.Helpers {
  public static class Extensions {
    static readonly ThreadLocal<Random> _rnd = new ThreadLocal<Random>(() => new Random());

    public static T GetRandom<T>(this T[] ar) => ar[_rnd.Value.Next(ar.Length)];

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
  }
}
