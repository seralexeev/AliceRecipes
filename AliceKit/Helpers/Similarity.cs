using System;
using System.Collections.Generic;
using System.Linq;
using F23.StringSimilarity;

namespace AliceKit.Helpers {
  public static class Similarity {
    public static (bool ok, TItem item) TryGetMostSimilar<TItem>(
      string input,
      IEnumerable<TItem> items,
      Func<TItem, string> selector,
      double minSimilarity = 0.2) {
      var nl = new NormalizedLevenshtein();
      var item = items.Select((x, i) => (similarity: nl.Similarity(selector(x), input), item: x))
        .Aggregate((i1, i2) => i1.similarity > i2.similarity ? i1 : i2);

      return item.similarity > minSimilarity ? (true, item.item) : default;
    }
  }
}
