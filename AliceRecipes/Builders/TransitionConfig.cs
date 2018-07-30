using System;
using AliceRecipes.Intents;
using AliceRecipes.Services;
using AliceRecipes.States;

namespace AliceRecipes.Builders {
  public class TransitionConfig<TState> {
    private readonly TState _state;
    private readonly ReplyBuilder _replyBuilder;

    public TransitionConfig(TState state, ReplyBuilder replyBuilder) {
      _state = state;
      _replyBuilder = replyBuilder;
    }

    public StateTransition To<TPhase>() where TPhase : PhaseBase<TState> => new StateTransition(typeof(TPhase), _state);
  }
}
