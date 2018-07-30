using AliceRecipes.States;

namespace AliceRecipes {
  public interface IStateManager {
    PhaseBase GetState(AliceRequest request);
  }
}
