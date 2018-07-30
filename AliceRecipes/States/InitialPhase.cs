using System;
using System.Threading;
using AliceRecipes.Helpers;
using AliceRecipes.Intents;

namespace AliceRecipes.States {
  public class InitialPhase : PhaseBase, IDefaultIntentHandler {
    static string[] WelcomePhrases = {
      "Привет, давно не виделись!",
      "Привет, меня зовут Василий и я днаю множество рецептов, о которых готов тебе рассказать!",
      "Привет, Давай знакомиться! Меня зовут Василий и я могу рассказть о рецептах.",
    };

    public HandleResult Handle() {
      var phrase = WelcomePhrases.GetRandom();
      var sayQuery = "Назови рецепт который хочешь найти!";

      return Reply(phrase + " " + sayQuery)
        .BigImageCard("1030494/9825443721439c9ba843", card => card.Title(sayQuery).Description(phrase))
        .Transition<PendingSearch>();
    }
  }
}
