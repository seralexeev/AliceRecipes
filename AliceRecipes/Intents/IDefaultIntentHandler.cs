using AliceRecipes.States;

namespace AliceRecipes.Intents {
  public interface IDefaultIntentHandler {
    HandleResult Handle();
  }

  public interface IIntentHandler<TIntent> {
    HandleResult Handle(TIntent intent);
  }

  public class HandleResult {
    public ResponseModel Response { get; set; }
    public StateTransition Transition { get; set; }
  }
}
