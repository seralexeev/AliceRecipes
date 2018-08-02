using System.Collections.Generic;
using AliceKit.Framework;

namespace AliceKit.Services {
  public class SessionManager {
    static Dictionary<string, ContextData> _db = new Dictionary<string, ContextData>();

    public (bool ok, ContextData) Get(string sessionId) =>
      _db.TryGetValue(sessionId, out var ctx) ? (true, ctx) : default;

    public void Set(ContextData ctx) => _db[ctx.SessionId] = ctx;
  }
}
