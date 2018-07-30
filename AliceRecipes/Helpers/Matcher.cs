using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace AliceRecipes.Helpers {
  public class Matcher<TResult> {
    readonly Dictionary<TResult, List<string>> _dict = new Dictionary<TResult, List<string>>();

    public IList<string> this[TResult key] =>
      _dict.TryGetValue(key, out var val) ? val : (_dict[key] = new List<string>());

    public (bool ok, TResult) Match(string str) {
      foreach (var (key, value) in _dict) {
        if (value.Any(x => str.IndexOf(x, StringComparison.InvariantCultureIgnoreCase) > -1)) {
          return (true, key);
        }
      }

      return (false, default);
    }
  }

  public class Matcher : IEnumerable<string> {
    readonly List<string> _list;

    public Matcher(IEnumerable<string> values = null) => _list = values?.ToList() ?? new List<string>();

    public void Add(string val) => _list.Add(val);
    public IEnumerator<string> GetEnumerator() => _list.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public bool Match(string str) => _list.Any(x => str.IndexOf(x, StringComparison.InvariantCultureIgnoreCase) > -1);
  }
}
