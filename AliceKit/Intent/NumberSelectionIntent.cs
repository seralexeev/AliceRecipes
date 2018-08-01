using AliceKit.Framework;

namespace AliceKit.Intent {
  public class NumberSelectionIntent : IntentBase {
    public int Number { get; }
    public NumberSelectionIntent(int number) => Number = number;
  }
}
