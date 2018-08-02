using System;
using AliceKit.Protocol;

namespace AliceKit.Builders {
  public class BigImageCardBuilder {
    readonly BigImageCard _card;

    public BigImageCardBuilder(string imageId) => _card = new BigImageCard(imageId);

    public BigImageCardBuilder Title(string title) => Set(x => x.Title = title);
    public BigImageCardBuilder Description(string description) => Set(x => x.Description = description);
    public BigImageCardBuilder Button(string text, string url = null) => Set(x => x.Button = new CardButton(text, url));

    public static implicit operator BigImageCard(BigImageCardBuilder replyBuilder) => replyBuilder._card;

    BigImageCardBuilder Set(Action<BigImageCard> act) {
      act(_card);
      return this;
    }
  }
}
