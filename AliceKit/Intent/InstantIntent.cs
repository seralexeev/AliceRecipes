using AliceKit.Framework;
using AliceKit.Protocol;

namespace AliceKit.Intent {
  public abstract class InstantIntent<TIntent> : IntentBase, IIntentMatcher, IIntentHandler<TIntent>
    where TIntent : IntentBase {
    (bool ok, IntentBase intent) IIntentMatcher.TryGetIntent(RequestModel req) {
      var (ok, intent) = TryGetIntent(req);
      return ok ? (true, intent as IntentBase) : default;
    }

    protected abstract (bool ok, TIntent intent) TryGetIntent(RequestModel req);
    public abstract HandleResult Handle(TIntent intent);
  }
}
