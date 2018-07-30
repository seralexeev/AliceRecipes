using System.Linq;
using AliceRecipes.Helpers;
using AliceRecipes.Intents;
using AliceRecipes.Services;
using static AliceRecipes.Constants.FixedList;

namespace AliceRecipes.States {
  public class PendingSearch : PhaseBase, IIntentHandler<CancelIntent>, IIntentHandler<SearchRequestIntent> {
    private readonly IRecipeService _service;

    public PendingSearch(IRecipeService service) => _service = service;

    protected override IntentBase GetIntent(RequestModel request) {
      if (CancelMather.Match(request.GetSatanized())) {
        return new CancelIntent();
      }

      return new SearchRequestIntent(request.GetSatanized());
    }

    public HandleResult Handle(CancelIntent intent) {
      return Reply("Ну ладно, заходи как захочешь что то приготовить")
        .EndSession()
        .BigImageCard("1030494/9825443721439c9ba843", card => card
          .Title("До скорой встречи")
          .Description("Заходи как захочешь что то приготовить, пока, пока!"));
    }

    public HandleResult Handle(SearchRequestIntent intent) {
      var searchResult = _service.Find(intent.Query).Result;
      if (searchResult.Items.Length == 0) {
        return Reply("К сожалению мне ничего не удалось найти, давай попробуем еще раз");
      }

      var state = new RecipeSelecting.StateType {
        Query = intent.Query,
        SearchResult = searchResult
      };

      return Reply($"По твоему запросу мне удалось найти {searchResult.Items.Length} рецептов")
        .ItemsListCard(card => card
          .Header("Вот что мне удалось найти:")
          .Items(state.SearchResult.Items.Select(x =>
            new ItemsListCardItem {
              Description = (x.Description ?? x.Note).TruncateBySentence(180),
              ImageId = x.AliceImageId,
              Title = x.Name.TruncateBySentence(180),
              Button = new CardButton(x.Name.TruncateBySentence(180))
            }).ToArray()))
        .Transition(state, x => x.To<RecipeSelecting>());
    }
  }
}
