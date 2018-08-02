using AliceKit.Framework;
using AliceKit.Protocol;
using static AliceKit.Helpers.FixedList;

namespace AliceKit.Intent.Matchers {
  public class CancelIntentMatcher : IIntentMatcher {
    public (bool ok, IntentBase intent) TryGetIntent(RequestModel req) =>
      CancelMather.Match(req.GetSanitized()) ? (true, new CancelIntent()) : default;
  }
}
