using System;
using System.Linq;
using AliceRecipes.Helpers;
using AliceRecipes.Intents;
using AliceRecipes.Models;
using AliceRecipes.Services;
using static AliceRecipes.Constants.FixedList;

namespace AliceRecipes.States {
  public class RecipeSelecting : PhaseBase<RecipeSelecting.StateType>,
    IIntentHandler<NumberSelectionIntent>,
    IIntentHandler<RecipeNameSelectionIntent>,
    IIntentHandler<CancelIntent> {
    private readonly IRecipeService _recipeService;
    private readonly RecipeInfoReply _recipeInfoReply;

    public RecipeSelecting(
      IRecipeService recipeService,
      RecipeInfoReply recipeInfoReply) {
      _recipeService = recipeService;
      _recipeInfoReply = recipeInfoReply;
    }

    protected override IntentBase GetIntent(RequestModel request) {
      if (CancelMather.Match(request.GetSatanized())) {
        return new CancelIntent();
      }

      var (ok, number) = NumberMather.Match(request.GetSatanized());
      if (ok) {
        return new NumberSelectionIntent(number);
      }

      return new RecipeNameSelectionIntent(request.GetSatanized());
    }

    public HandleResult Handle(NumberSelectionIntent intent) {
      var item = State.SearchResult.Items.Skip(intent.Number - 1).FirstOrDefault();
      return SelectionReply(item);
    }

    public HandleResult Handle(CancelIntent intent) =>
      Reply("Хорошо, давай попробуем найти что то другое")
        .Transition<PendingSearch>();

    public HandleResult Handle(RecipeNameSelectionIntent intent) {
      var item = State.SearchResult.Items.FirstOrDefault(x =>
        x.Name.Equals(intent.Name, StringComparison.InvariantCultureIgnoreCase));

      return SelectionReply(item);
    }

    private HandleResult SelectionReply(RecipePreview item) {
      if (item == null) {
        return Reply("Я не очень тебя понял, выбери пожалуйста рецепт из списка или скажи отмена")
          .ItemsListCard(card => card
            .Header("Вот что мне удалось найти:")
            .Items(State.SearchResult.Items, (x, builder) => builder
              .Title(x.Name)
              .Description(x.Description)
              .ImageId(x.AliceImageId)
              .Button(x.Name)))
          .Transition(State, x => x.To<RecipeSelecting>());
      }

      var recipe = _recipeService.Get(item.Id).Result;
      if (recipe == null) {
        return Reply("К сожалению не удалось найти этот рецепт, давай попробуем еще раз")
          .Transition<PendingSearch>();
      }

      return _recipeInfoReply.RecipeInfo("Отличный выбор", recipe).Transition(new RecipeSelected.StateType {
        Recipe = recipe
      }, x => x.To<RecipeSelected>());
    }

    public class StateType {
      public string Query { get; set; }
      public QueryResult<RecipePreview> SearchResult { get; set; }
    }
  }
}
