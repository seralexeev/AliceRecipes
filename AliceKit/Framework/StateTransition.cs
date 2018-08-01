namespace AliceKit.Framework {
  public class StateTransition {
    public StateTransition(string name, object state) {
      Name = name;
      State = state;
    }

    public string Name { get; }
    public object State { get; }
  }
}
