using System.Linq;
using AliceKit.Builders;
using AliceKit.Framework;
using AliceKit.Intent;
using AliceKit.Protocol;
using AliceKit.Services;
using AliceRecipes.Intents;
using AliceRecipes.Services;

namespace AliceRecipes.Blocks {
  public class PendingSearchBlock : BlockBase,
    IIntentHandler<CancelIntent>,
    IIntentHandler<SearchRequestIntent> {
    readonly IRecipeService _service;
    private readonly IImageService _imageService;

    public PendingSearchBlock(IRecipeService service, IImageService imageService) {
      _service = service;
      _imageService = imageService;
    }

    public override (bool ok, IntentBase intent) TryGetIntent(RequestModel req) {
      return (true, new SearchRequestIntent(req.GetSanitized(
        "я", "попросить", "тебя", "найди", "найти", "мне", "много",
        "поищи", "давай", "поищем", "найдем", "дай", "покажи", "запусти",
        "рецепт", "рецепты", "выдай", "быстро", "хочу", "рецептов")));
    }

    public HandleResult Handle(CancelIntent intent) {
      return ReplyBuilder.Reply("Ну ладно, заходи как захочешь что то приготовить")
        .EndSession()
        .BigImageCard("1030494/9825443721439c9ba843", card => card
          .Title("До скорой встречи")
          .Description("Заходи как захочешь что то приготовить, пока, пока!"))
        .Transition<InitialBlock>();
    }

    public HandleResult Handle(SearchRequestIntent intent) {
      var searchResult = _service.Find(intent.Query).Result;
      if (searchResult.Items.Length == 0) {
        return ReplyBuilder.Reply("К сожалению мне ничего не удалось найти, давай попробуем еще раз");
      }

      var state = new RecipeSelectingBlock.StateType {
        Query = intent.Query,
        SearchResult = searchResult
      };

      return ReplyBuilder.Reply($"По твоему запросу мне удалось найти {searchResult.Items.Length} рецептов")
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
