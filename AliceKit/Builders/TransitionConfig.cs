using AliceKit.Framework;

namespace AliceKit.Builders {
  public class TransitionConfig<TState> {
    private readonly TState _state;
    public TransitionConfig(TState state) => _state = state;
    public StateTransition To<TPhase>() where TPhase : BlockBase<TState> =>
      new StateTransition(typeof(TPhase).Name, _state);
  }
}
