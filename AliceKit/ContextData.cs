using AliceKit.Protocol;

namespace AliceKit {
  public class ContextData {
    public string SessionId { get; set; }
    public string BlockName { get; set; }
    public object State { get; set; }
    public SessionModel Session { get; set; }
  }
}
