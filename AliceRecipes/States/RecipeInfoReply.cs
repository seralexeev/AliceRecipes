using System;
using System.Collections.Generic;
using System.Linq;
using AliceRecipes.Builders;
using AliceRecipes.Helpers;
using AliceRecipes.Intents;
using AliceRecipes.Models;
using static System.StringComparer;
using static AliceRecipes.States.RecipeInfoReply.Consts;

namespace AliceRecipes.States {
  public class RecipeInfoReply {
    public static class Consts {
      public const string ABOUT = "О рецепте";
      public const string INGREDIENTS = "Ингредиенты";
      public const string STEPS = "Способ приготовления";
      public const string IMAGES = "Фотографии";
    }

    public static readonly ButtonModel[] DefaultButtons = new[] {
      ABOUT,
      INGREDIENTS,
      STEPS,
      IMAGES
    }.Select(x => new ButtonModel(x)).ToArray();

    private readonly Dictionary<string, Func<Recipe, ReplyBuilder>> _actions;

    public RecipeInfoReply() {
      _actions = new Dictionary<string, Func<Recipe, ReplyBuilder>>(CurrentCultureIgnoreCase) {
        [ABOUT] = x => RecipeInfo(x.Name, x),
        [INGREDIENTS] = Ingredients,
        [STEPS] = Steps,
        [IMAGES] = Images,
      };
    }

    public ReplyBuilder RecipeInfo(string reply, Recipe recipe) {
      return new ReplyBuilder(reply).BigImageCard(recipe.AliceImageId, card => card
          .Title(recipe.Name.TruncateBySentence(180))
          .Button(ABOUT)
          .Description((recipe.Description ?? recipe.Note).TruncateBySentence(180)))
        .Buttons(DefaultButtons);
    }

    public ReplyBuilder Ingredients(Recipe recipe) {
      return new ReplyBuilder(string.Join("\n", new[] {1, 2, 3}.Select(x => $"{x}. Ингредиент")))
        .Buttons(DefaultButtons);
    }

    public ReplyBuilder Steps(Recipe recipe) {
      return new ReplyBuilder(string.Join("\n", new[] {1, 2, 3}.Select(x => $"{x}. Шаги")))
        .Buttons(DefaultButtons);
    }

    public ReplyBuilder Images(Recipe recipe) {
      return new ReplyBuilder(string.Join("\n", new[] {1, 2, 3}.Select(x => $"{x}. Фотки")))
        .Buttons(DefaultButtons);
    }

    public ButtonPressIntent GetIntent(RequestModel req) {
      var text = req.GetOriginal();
      if (_actions.ContainsKey(text)) {
        return new ButtonPressIntent(text);
      }

      return null;
    }

    public (bool ok, ReplyBuilder builder) TryHandle(ButtonPressIntent intent, Recipe recipe) {
      if (_actions.TryGetValue(intent.Text, out var act)) {
        return (true, act(recipe));
      }

      return (false, null);
    }
  }
}
