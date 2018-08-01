using System.Collections.Generic;

namespace AliceKit {
  public class SessionManager {
    static Dictionary<string, ContextData> _db = new Dictionary<string, ContextData>();

    public (bool ok, ContextData) Get(string sessionId) {
      return _db.TryGetValue(sessionId, out var ctx) ? (true, ctx) : default;
    }

    public void Set(ContextData ctx) => _db[ctx.SessionId] = ctx;
  }
}
