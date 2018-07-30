using AliceRecipes.Intents;
using AliceRecipes.Models;

namespace AliceRecipes.States {
  public class RecipeSelected : PhaseBase<RecipeSelected.StateType>,
    IIntentHandler<ButtonPressIntent> {
    private readonly RecipeInfoReply _recipeInfoReply;

    public RecipeSelected(RecipeInfoReply recipeInfoReply) {
      _recipeInfoReply = recipeInfoReply;
    }

    public HandleResult Handle(ButtonPressIntent intent) {
      var (ok, builder) = _recipeInfoReply.TryHandle(intent, State.Recipe);
      if (ok) {
        return builder;
      }

      return null;
    }

    protected override IntentBase GetIntent(RequestModel request) {
      return _recipeInfoReply.GetIntent(request);
    }

    public class StateType {
      public Recipe Recipe { get; set; }
    }
  }
}
