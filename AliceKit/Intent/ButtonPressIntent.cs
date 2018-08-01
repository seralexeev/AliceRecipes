using AliceKit.Framework;

namespace AliceKit.Intent {
  public class ButtonPressIntent : IntentBase {
    public string Text { get; }
    public ButtonPressIntent(string text) => Text = text;
  }
}
