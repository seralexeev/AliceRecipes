using System;
using System.Collections.Generic;
using System.Linq;
using AliceKit.Builders;
using AliceKit.Framework;
using AliceKit.Helpers;
using AliceKit.Intent;
using AliceKit.Protocol;
using AliceRecipes.Models;
using static System.StringComparer;
using static AliceRecipes.States.RecipeInfoReply.Consts;

namespace AliceRecipes.States {
  public class RecipeInfoReply : IIntentMatcher {
    public static class Consts {
      public const string ABOUT = "О рецепте";
      public const string INGREDIENTS = "Ингредиенты";
      public const string STEPS = "Способ приготовления";
      public const string IMAGES = "Фотографии";
      public const string CANCEL = "Отмена";
    }

    public static readonly ButtonModel[] DefaultButtons = new[] {
      ABOUT,
      INGREDIENTS,
      STEPS,
      IMAGES,
      CANCEL
    }.Select(x => new ButtonModel(x)).ToArray();

    private readonly Dictionary<string, Func<Recipe, ReplyBuilder>> _actions;

    public RecipeInfoReply() {
      _actions = new Dictionary<string, Func<Recipe, ReplyBuilder>>(CurrentCultureIgnoreCase) {
        [ABOUT] = x => RecipeInfo(x.Name, x),
        [INGREDIENTS] = Ingredients,
        [STEPS] = Steps,
        [IMAGES] = Images
      };
    }

    public ReplyBuilder RecipeInfo(string reply, Recipe recipe) {
      return Reply(reply).BigImageCard(recipe.AliceImageId ?? "1030494/9825443721439c9ba843", card => card
          .Title(recipe.Name)
          .Button(ABOUT)
          .Description((recipe.Description ?? recipe.Note)))
        .Buttons(DefaultButtons);
    }

    ReplyBuilder Ingredients(Recipe recipe) {
      return Reply(string.Join("\n", new[] {1, 2, 3}.Select(x => $"{x}. Ингредиент")))
        .Buttons(DefaultButtons);
    }

    ReplyBuilder Steps(Recipe recipe) {
      return Reply(string.Join("\n", new[] {1, 2, 3}.Select(x => $"{x}. Шаги")))
        .Buttons(DefaultButtons);
    }

    ReplyBuilder Images(Recipe recipe) {
      return Reply(string.Join("\n", new[] {1, 2, 3}.Select(x => $"{x}. Фотки")))
        .Buttons(DefaultButtons);
    }

    public (bool ok, ReplyBuilder builder) TryHandle(ButtonPressIntent intent, Recipe recipe) {
      if (_actions.TryGetValue(intent.Text, out var act)) {
        return (true, act(recipe));
      }

      return (false, null);
    }

    public (bool ok, IntentBase intent) TryGetIntent(RequestModel req) {
      var text = req.GetOriginal();
      var (ok, button) = Similarity.TryGetMostSimilar(text, DefaultButtons, x => x.Title);
      return ok ? (true, new ButtonPressIntent(button.Title)) : default;
    }

    private ReplyBuilder Reply(string text) => new ReplyBuilder(text);
  }
}
