namespace AliceKit.Framework {
  public interface IIntentHandler<TIntent> {
    HandleResult Handle(TIntent intent);
  }
}
