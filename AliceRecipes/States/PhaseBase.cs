using System;
using System.Collections.Generic;
using System.Linq;
using AliceRecipes.Builders;
using AliceRecipes.Intents;

namespace AliceRecipes.States {
  public interface IStatefulPhase {
    object State { get; set; }
  }

  public abstract class PhaseBase<TStateData> : PhaseBase, IStatefulPhase {
    protected TStateData State => (TStateData) (this as IStatefulPhase).State;
    object IStatefulPhase.State { get; set; }
  }

  public abstract class PhaseBase {
    public string Name => GetType().Name;

    protected ReplyBuilder Reply(string reply) => new ReplyBuilder(reply);

    public HandleResult Handle(RequestModel request) {
      var intent = GetIntent(request);
      Console.WriteLine($"Handle by {Name} -> {(intent != null ? intent.ToString() : "NULL_INTENT")}");
      return HandleIntent(intent);
    }

    private HandleResult HandleIntent(IntentBase intent) {
      if (intent == null) {
        if (this is IDefaultIntentHandler handler) {
          return handler.Handle();
        }

        throw new Exception("Unable to handle intent");
      }

      var intentHandlerType = typeof(IIntentHandler<>).MakeGenericType(intent.GetType());
      if (!intentHandlerType.IsInstanceOfType(this)) {
        if (this is IDefaultIntentHandler handler) {
          return handler.Handle();
        }

        throw new Exception("Unable to handle intent");
      }

      var mi = intentHandlerType.GetMethod(nameof(IIntentHandler<IntentBase>.Handle));
      return mi.Invoke(this, new object[] {intent}) as HandleResult;
    }

    protected virtual IntentBase GetIntent(RequestModel request) => null;
  }
}
