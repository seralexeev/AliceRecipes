using AliceRecipes.Helpers;

namespace AliceRecipes.Constants {
  public static class FixedList {
    public static readonly string[] CancelWords = {
      "хватит",
      "стоп",
      "назад",
      "прекрати",
      "вернись",
      "уйди",
      "выключить",
      "выключись",
      "выход",
      "выйди",
      "домой",
      "отмена",
      "отмени",
      "другое"
    };

    public static readonly string[] GarbageWords = {
      "эм",
      "хочу",
      "давай",
      "ну",
      "алиса",
      "мне",
      "ладно",
    };

    public static readonly Matcher CancelMather = new Matcher(CancelWords);

    public static readonly Matcher<int> NumberMather = new Matcher<int> {
      [1] = {"1", "один", "первый", "первая", "первую", "номер один"},
      [2] = {"2", "два", "второй", "вторая", "вторую", "номер два"},
      [3] = {"3", "три", "третий", "третья", "третию", "номер три"},
      [4] = {"4", "четыре", "четвертый", "четвертая", "четвертую", "номер четые"},
      [5] = {"5", "пять", "пятый", "пятая", "пятую", "номер пять"},
    };
  }
}
