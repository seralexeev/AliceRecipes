using AliceKit.Builders;
using AliceKit.Framework;
using AliceKit.Intent;
using AliceKit.Protocol;
using AliceRecipes.Models;

namespace AliceRecipes.Blocks {
  public class RecipeSelectedBlock : BlockBase<RecipeSelectedBlock.StateType>,
    IIntentHandler<ButtonPressIntent>,
    IIntentHandler<CancelIntent> {
    readonly RecipeInfoReply _recipeInfoReply;

    public RecipeSelectedBlock(RecipeInfoReply recipeInfoReply) {
      _recipeInfoReply = recipeInfoReply;
    }

    public HandleResult Handle(ButtonPressIntent intent) {
      var (ok, builder) = _recipeInfoReply.TryHandle(intent, State.Recipe);
      return ok ? builder : base.Handle(new UnknownIntent());
    }

    public override (bool ok, IntentBase intent) TryGetIntent(RequestModel req) => _recipeInfoReply.TryGetIntent(req);

    public HandleResult Handle(CancelIntent intent) =>
      ReplyBuilder.Reply("Хорошо, давай попробуем найти что то другое, назови рецепт который хочешь найти")
        .BigImageCard("1030494/9825443721439c9ba843", card => card
          .Title("Назови рецепт который хочешь найти")
          .Description("И я найду его в моей огромной базе рецептов"))
        .Transition<PendingSearchBlock>();

    public class StateType {
      public Recipe Recipe { get; set; }
    }
  }
}
