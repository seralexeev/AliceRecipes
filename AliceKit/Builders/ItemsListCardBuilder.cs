using System;
using System.Collections.Generic;
using System.Linq;
using AliceKit.Protocol;

namespace AliceKit.Builders {
  public class ItemsListCardBuilder {
    readonly ItemsListCard _card;

    public ItemsListCardBuilder() => _card = new ItemsListCard();

    public ItemsListCardBuilder Header(string title) => Set(x => x.Header = new ItemsListCardHeader(title));
    public ItemsListCardBuilder Footer(string title) => Set(x => x.Footer = new ItemsListCardFooter(title));

    public ItemsListCardBuilder Footer(string title, string buttonText, string url = null) =>
      Set(x => x.Footer = new ItemsListCardFooter(title) {
        Button = new CardButton(buttonText, url)
      });

    public ItemsListCardBuilder Items(params ItemsListCardItem[] items) => Set(x => x.Items = items);

    public ItemsListCardBuilder Items(Action<ItemsBuilder> func) {
      var itemsBuilder = new ItemsBuilder();
      func(itemsBuilder);
      return Set(x => x.Items = itemsBuilder.Items.Select(t => t.CardItem).ToArray());
    }

    public ItemsListCardBuilder Items<T>(IEnumerable<T> items,
      Func<T, ItemsListCardItemBuilder, ItemsListCardItemBuilder> itemBuilder) =>
      Items(items, (x, _, builder) => itemBuilder(x, builder));

    public ItemsListCardBuilder Items<T>(IEnumerable<T> items,
      Func<T, int, ItemsListCardItemBuilder, ItemsListCardItemBuilder> itemBuilder) {
      return Set(x =>
        x.Items = items.Select((item, i) => itemBuilder(item, i, new ItemsListCardItemBuilder()).CardItem).ToArray());
    }

    public static implicit operator ItemsListCard(ItemsListCardBuilder replyBuilder) => replyBuilder._card;

    ItemsListCardBuilder Set(Action<ItemsListCard> act) {
      act(_card);
      return this;
    }
  }

  public class ItemsBuilder {
    public List<ItemsListCardItemBuilder> Items { get; } = new List<ItemsListCardItemBuilder>();

    public ItemsBuilder Add(Action<ItemsListCardItemBuilder> func) {
      var builder = new ItemsListCardItemBuilder();
      func(builder);
      Items.Add(builder);
      return this;
    }
  }

  public class ItemsListCardItemBuilder {
    public ItemsListCardItem CardItem { get; } = new ItemsListCardItem();

    public ItemsListCardItemBuilder Title(string title) => Set(x => x.Title = title);
    public ItemsListCardItemBuilder ImageId(string imageId) => Set(x => x.ImageId = imageId);
    public ItemsListCardItemBuilder Description(string description) => Set(x => x.ImageId = description);

    public ItemsListCardItemBuilder Button(string text, string url = null) =>
      Set(x => x.Button = new CardButton(text, url));

    ItemsListCardItemBuilder Set(Action<ItemsListCardItem> act) {
      act(CardItem);
      return this;
    }
  }
}
