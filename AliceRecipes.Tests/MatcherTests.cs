using System;
using AliceRecipes.Helpers;
using Xunit;

namespace AliceRecipes.Tests {
  public class MatcherTests {
    [Theory]
    [InlineData("Открой первый объект", new[] {"1", "первый", "первая", "первый"}, 1)]
    public void MatchIntResult(string expected, string[] cases, int expectedResult) {
      var (ok, result) = new Matcher<int> {
        [1] = {"1", "первый", "первая", "первый"},
      }.Match("Открой первый рецепт");

      Assert.True(ok);
      Assert.Equal(expectedResult, result);
    }
  }
}
