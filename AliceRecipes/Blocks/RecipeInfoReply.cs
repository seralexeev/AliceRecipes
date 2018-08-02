using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using AliceKit.Builders;
using AliceKit.Framework;
using AliceKit.Helpers;
using AliceKit.Intent;
using AliceKit.Protocol;
using AliceRecipes.Models;
using static System.String;
using static AliceKit.Builders.ReplyBuilder;

namespace AliceRecipes.Blocks {
  public class RecipeInfoReply : BlockBase, IIntentMatcher {
    static class Consts {
      public const string ABOUT = "О рецепте";
      public const string INGREDIENTS = "Ингредиенты";
      public const string STEPS = "Способ приготовления";
      public const string CANCEL = "Отмена";
    }

    static readonly ButtonModel[] DefaultButtons = new[] {
      Consts.ABOUT,
      Consts.INGREDIENTS,
      Consts.STEPS,
      Consts.CANCEL
    }.Select(x => new ButtonModel(x)).ToArray();

    readonly Dictionary<string, Func<Recipe, ReplyBuilder>> _actions;

    public RecipeInfoReply() {
      _actions = new Dictionary<string, Func<Recipe, ReplyBuilder>>(StringComparer.CurrentCultureIgnoreCase) {
        [Consts.ABOUT] = x => RecipeInfo(x.Name, x),
        [Consts.INGREDIENTS] = Ingredients,
        [Consts.STEPS] = Steps
      };
    }

    public ReplyBuilder RecipeInfo(string reply, Recipe recipe) {
      return Reply(reply).BigImageCard(recipe.AliceImageId ?? "1030494/9825443721439c9ba843", card => card
          .Title(recipe.Name)
          .Button(Consts.ABOUT)
          .Description((recipe.Description ?? recipe.Note)))
        .Buttons(DefaultButtons);
    }

    ReplyBuilder Ingredients(Recipe recipe) {
      var reply = $"Для приготовления {recipe.PortionsCount} порций, вам потребуется:\n\n";
      var ingredients = Join("\n",
        recipe.Ingredients.Items.Select((x, i) => $"{PadLeft(i + 1)}. {FormatIngredient(x)}"));

      return Reply(reply + ingredients)
        .Buttons(DefaultButtons);
    }

    ReplyBuilder Steps(Recipe recipe) {
      var reply = $"Для приготовления {recipe.Name}, вам необходимо:\n\n";
      var steps = Join("\n", recipe.Steps.Items.Select((x, i) => $"{PadLeft(i + 1)}. {x.Description}"));

      return Reply(reply + steps).Buttons(DefaultButtons);
    }

    string FormatIngredient(IngredientNode ingredient) {
      var amount = double.Parse(ingredient.Amount);
      var amountStr = amount != 0 ? amount.ToString(CultureInfo.InvariantCulture) : "";
      return $"{ingredient.Item.Name}, {amountStr} {ingredient.Item.Measure}";
    }

    public (bool ok, ReplyBuilder builder) TryHandle(ButtonPressIntent intent, Recipe recipe) =>
      _actions.TryGetValue(intent.Text, out var act) ? (true, act(recipe)) : default;

    public (bool ok, IntentBase intent) TryGetIntent(RequestModel req) {
      var text = req.GetOriginal();
      var (ok, button) = Similarity.TryGetMostSimilar(text, DefaultButtons, x => x.Title);
      return ok ? (true, new ButtonPressIntent(button.Title)) : default;
    }

    string PadLeft(int n, int count = 2) => n.ToString().PadLeft(count, ' ');
  }
}
