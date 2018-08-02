using AliceKit.Framework;
using AliceKit.Protocol;

namespace AliceKit.Intent.Matchers {
  public class UnknownIntentMatcher : IIntentMatcher {
    public (bool ok, IntentBase intent) TryGetIntent(RequestModel req) => (true, new UnknownIntent());
  }
}
