using AliceKit.Intent;
using AliceKit.Protocol;
using static AliceKit.Builders.ReplyBuilder;

namespace AliceKit.Framework {
  public abstract class BlockBase : IIntentHandler<UnknownIntent>, IIntentMatcher {
    public string Name => GetType().Name;

    public virtual (bool ok, IntentBase intent) TryGetIntent(RequestModel req) => default;

    public virtual HandleResult Handle(UnknownIntent intent) => Reply("Я пока не знаю как ответить на такой запрос");
  }

  public abstract class BlockBase<TStateData> : BlockBase, IStatefulBlock {
    protected TStateData State => (TStateData) (this as IStatefulBlock).State;
    object IStatefulBlock.State { get; set; }
  }
}
