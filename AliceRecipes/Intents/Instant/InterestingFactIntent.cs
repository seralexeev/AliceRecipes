using AliceKit.Builders;
using AliceKit.Framework;
using AliceKit.Helpers;
using AliceKit.Intent;
using AliceKit.Protocol;

namespace AliceRecipes.Intents.Instant {
  public class InterestingFactIntent : InstantIntent<InterestingFactIntent> {
    static readonly string[] WelcomePhrases = {
      "Привет, давно не виделись!",
      "Привет, меня зовут Василий и я днаю множество рецептов, о которых готов тебе рассказать!",
      "Привет, Давай знакомиться! Меня зовут Василий и я могу рассказть о рецептах."
    };

    protected override (bool ok, InterestingFactIntent intent) TryGetIntent(RequestModel req) =>
      req.Contains("привет", "здарова", "здравствуй") ? (true, new InterestingFactIntent()) : default;

    public override HandleResult Handle(InterestingFactIntent intent) => ReplyBuilder.Reply(WelcomePhrases.GetRandom());
  }
}
