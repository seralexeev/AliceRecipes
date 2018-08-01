using AliceKit.Framework;

namespace AliceRecipes.Intents {
  public class ItemSelectionIntent<TItem> : IntentBase {
    public TItem Item { get; }
    public ItemSelectionIntent(TItem item) => Item = item;
  }
}
