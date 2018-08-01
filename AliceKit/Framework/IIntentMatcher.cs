using AliceKit.Protocol;

namespace AliceKit.Framework {
  public interface IIntentMatcher {
    (bool ok, IntentBase intent) TryGetIntent(RequestModel req);
  }
}
