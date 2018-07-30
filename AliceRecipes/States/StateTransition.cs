using System;

namespace AliceRecipes.States {
  public class StateTransition {
    public StateTransition(Type type, object stateData) {
      Type = type;
      StateData = stateData;
    }

    public Type Type { get; }
    public object StateData { get; }
  }
}
