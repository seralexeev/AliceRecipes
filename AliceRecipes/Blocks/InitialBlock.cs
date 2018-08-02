using AliceKit.Builders;
using AliceKit.Framework;
using AliceKit.Helpers;
using AliceKit.Intent;

namespace AliceRecipes.Blocks {
  public class InitialBlock : BlockBase {
    static readonly string[] WelcomePhrases = {
      "Привет, давно не виделись!",
      "Привет, меня зовут Василий и я днаю множество рецептов, о которых готов тебе рассказать!",
      "Привет, Давай знакомиться! Меня зовут Василий и я могу рассказть о рецептах."
    };

    public override HandleResult Handle(UnknownIntent intent) {
      var welcome = WelcomePhrases.GetRandom();
      var reply = "Назови рецепт который хочешь найти!";

      return ReplyBuilder.Reply(welcome + " " + reply)
        .BigImageCard("1030494/9825443721439c9ba843", card => card
          .Title(reply).Description(welcome))
        .Transition<PendingSearchBlock>();
    }
  }
}
