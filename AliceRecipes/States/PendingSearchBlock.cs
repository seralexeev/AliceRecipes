using AliceKit.Framework;
using AliceKit.Intent;
using AliceKit.Protocol;
using AliceRecipes.Intents;
using AliceRecipes.Services;
using static AliceKit.Builders.ReplyBuilder;

namespace AliceRecipes.States {
  public class PendingSearchBlock : BlockBase,
    IIntentHandler<CancelIntent>,
    IIntentHandler<SearchRequestIntent> {
    private readonly IRecipeService _service;
    public PendingSearchBlock(IRecipeService service) => _service = service;

    public override (bool ok, IntentBase intent) TryGetIntent(RequestModel req) {
      return (true,
        new SearchRequestIntent(req.GetSanitized("найди", "найти", "мне", "поищи", "давай", "поищем", "найдем", "дай", "покажи",
          "рецепт", "рецепты", "выдай", "быстро", "хочу", "рецептов", "много", "запусти")));
    }

    public HandleResult Handle(CancelIntent intent) {
      return Reply("Ну ладно, заходи как захочешь что то приготовить")
        .EndSession()
        .BigImageCard("1030494/9825443721439c9ba843", card => card
          .Title("До скорой встречи")
          .Description("Заходи как захочешь что то приготовить, пока, пока!"))
        .Transition<InitialBlock>();
    }

    public HandleResult Handle(SearchRequestIntent intent) {
      var searchResult = _service.Find(intent.Query).Result;
      if (searchResult.Items.Length == 0) {
        return Reply("К сожалению мне ничего не удалось найти, давай попробуем еще раз");
      }

      var state = new RecipeSelectingBlock.StateType {
        Query = intent.Query,
        SearchResult = searchResult
      };

      return Reply($"По твоему запросу мне удалось найти {searchResult.Items.Length} рецептов")
        .ItemsListCard(card => card
          .Header($"Вот что мне удалось найти по запросу \"{intent.Query}\":")
          .Items(searchResult.Items, (x, i, builder) => builder
            .Title($"{i + 1}. {x.Name}")
            .Description(x.Description)
            .ImageId(x.AliceImageId ?? "1030494/9825443721439c9ba843")
            .Button(x.Name)))
        .Transition(state, x => x.To<RecipeSelectingBlock>());
    }
  }
}
