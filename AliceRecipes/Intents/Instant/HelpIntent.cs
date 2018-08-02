using AliceKit.Builders;
using AliceKit.Framework;
using AliceKit.Helpers;
using AliceKit.Intent;
using AliceKit.Protocol;

namespace AliceRecipes.Intents.Instant {
  public class HelpIntent : InstantIntent<HelpIntent> {
    protected override (bool ok, HelpIntent intent) TryGetIntent(RequestModel req) =>
      req.Contains("помощь", "помоги", "что ты умеешь") ? (true, new HelpIntent()) : default;

    public override HandleResult Handle(HelpIntent intent) =>
      ReplyBuilder.Reply("Меня зовут Василий и я знаю множество рецептов, о которых готов тебе рассказать!")
        .ItemsListCard(card => card
          .Header("Вот список того что я умею")
          .Items(items => items
            .Add(item => item
              .Title("Искать рецепты")
              .ImageId("1030494/9825443721439c9ba843"))
            .Add(item => item
              .Title("Показывать список ингредиентов")
              .ImageId("1030494/9825443721439c9ba843"))
            .Add(item => item
              .Title("Помогать в приготовлении")
              .ImageId("1030494/9825443721439c9ba843"))));
  }
}
