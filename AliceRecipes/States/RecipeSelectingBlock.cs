using System.Linq;
using AliceKit.Builders;
using AliceKit.Framework;
using AliceKit.Helpers;
using AliceKit.Intent;
using AliceKit.Protocol;
using AliceRecipes.Intents;
using AliceRecipes.Models;
using AliceRecipes.Services;
using static AliceKit.Builders.ReplyBuilder;

namespace AliceRecipes.States {
  public class RecipeSelectingBlock : BlockBase<RecipeSelectingBlock.StateType>,
    IIntentHandler<NumberSelectionIntent>,
    IIntentHandler<CancelIntent>,
    IIntentHandler<ItemSelectionIntent<RecipePreview>> {
    private readonly IRecipeService _recipeService;
    private readonly RecipeInfoReply _recipeInfoReply;

    public RecipeSelectingBlock(
      IRecipeService recipeService,
      RecipeInfoReply recipeInfoReply) {
      _recipeService = recipeService;
      _recipeInfoReply = recipeInfoReply;
    }

    public override (bool ok, IntentBase intent) TryGetIntent(RequestModel req) {
      var text = req.GetSanitized();
      var (ok, item) = Similarity.TryGetMostSimilar(text, State.SearchResult.Items, x => x.Name);
      return ok ? (true, new ItemSelectionIntent<RecipePreview>(item)) : default;
    }

    public HandleResult Handle(NumberSelectionIntent intent) {
      var item = State.SearchResult.Items.Skip(intent.Number - 1).FirstOrDefault();
      return SelectionReply(item);
    }

    public HandleResult Handle(CancelIntent intent) =>
      Reply("Хорошо, давай попробуем найти что то другое")
        .Transition<PendingSearchBlock>();

    public HandleResult Handle(ItemSelectionIntent<RecipePreview> intent) => SelectionReply(intent.Item);

    private HandleResult SelectionReply(RecipePreview item) {
      if (item == null) {
        return Unkown();
      }

      var recipe = _recipeService.Get(item.Id).Result;
      if (recipe == null) {
        return Unkown().Transition<PendingSearchBlock>();
      }

      return _recipeInfoReply.RecipeInfo("Отличный выбор", recipe).Transition(new RecipeSelectedBlock.StateType {
        Recipe = recipe
      }, x => x.To<RecipeSelectedBlock>());
    }

    public override HandleResult Handle(UnknownIntent intent) => Unkown();

    private ReplyBuilder Unkown() => Reply("Я не очень тебя понял, выбери пожалуйста рецепт из списка или скажи отмена")
      .ItemsListCard(card => card
        .Header("Вот что мне удалось найти:")
        .Items(State.SearchResult.Items, (x, i, builder) => builder
          .Title($"{i}. {x.Name}")
          .Description(x.Description)
          .ImageId(x.AliceImageId ?? "1030494/9825443721439c9ba843")
          .Button(x.Name)))
      .Transition(State, x => x.To<RecipeSelectingBlock>());

    public class StateType {
      public string Query { get; set; }
      public QueryResult<RecipePreview> SearchResult { get; set; }
    }
  }
}
