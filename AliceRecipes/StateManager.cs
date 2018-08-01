using AliceKit.Framework;
using AliceKit.Protocol;

namespace AliceRecipes {
  public interface IStateManager {
    BlockBase GetState(AliceRequest request);
  }
}
