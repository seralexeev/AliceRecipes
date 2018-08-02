using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AliceKit.Helpers;
using AliceKit.Intent;
using AliceKit.Intent.Matchers;
using AliceKit.Protocol;
using AliceKit.Services;
using Newtonsoft.Json;

namespace AliceKit.Framework {
  public class Processor {
    readonly UnknownIntentMatcher _unknownIntentMatcher = new UnknownIntentMatcher();
    readonly IBlockFactory _blockFactory;
    readonly string _defaultBlockName;
    readonly ProcessorOptions _options;
    readonly SessionManager _sessionManager = new SessionManager();

    readonly MethodInfo _handleGenericIntentMethodInfo =
      typeof(Processor).GetMethod(nameof(HandleGenericIntent), BindingFlags.Instance | BindingFlags.NonPublic);

    public Processor(IBlockFactory blockFactory, string defaultBlockName, ProcessorOptions options = null) {
      _blockFactory = blockFactory;
      _defaultBlockName = defaultBlockName;
      _options = options ?? new ProcessorOptions();
    }

    public AliceResponse Process(AliceRequest req) {
      try {
        var ctx = GetContextData(req);
        var result = Process(req, _blockFactory.CreateBlock(ctx.BlockName, ctx.State));
        SetContextData(result, ctx);

        return new AliceResponse {
          Response = result.Response,
          Session = req.Session
        };
      }
      catch (Exception e) {
        Log(e);

        return new AliceResponse {
          Response = _options.ErrorResponse ?? new ResponseModel("Что то пошло не так"),
          Session = req.Session
        };
      }
    }

    ContextData GetContextData(AliceRequest req) {
      var (ok, ctx) = _sessionManager.Get(req.Session.SessionId);
      if (!ok) {
        ctx = new ContextData {
          BlockName = _defaultBlockName,
          Session = req.Session,
          SessionId = req.Session.SessionId
        };
      }

      return ctx;
    }

    void SetContextData(HandleResult result, ContextData ctx) => _sessionManager.Set(new ContextData {
      BlockName = result.Transition != null ? result.Transition.Name : ctx.BlockName,
      State = result.Transition != null ? result.Transition.State : ctx.State,
      Session = ctx.Session,
      SessionId = ctx.SessionId
    });

    IEnumerable<IIntentMatcher> GetIntents(IIntentMatcher block) {
      if (_options.CommonIntentMatchers != null) {
        foreach (var intent in _options.CommonIntentMatchers) {
          yield return intent;
        }
      }

      yield return new CancelIntentMatcher();
      yield return block;
      yield return _unknownIntentMatcher;
    }

    HandleResult Process(AliceRequest req, BlockBase block) {
      var intent = GetIntents(block)
        .Select(x => x.TryGetIntent(req.Request))
        .Where(x => x.ok)
        .Select(x => x.intent)
        .FirstOrDefault();

      Log($"Intent: {JsonConvert.SerializeObject(intent, Formatting.Indented)}");

      if (intent.IsInstantIntent()) {
        var (okInstant, resultInstant) = HandleIntent(intent, intent);
        if (okInstant) {
          return resultInstant;
        }
      }

      var (ok, result) = HandleIntent(block, intent);
      if (ok) {
        return result;
      }

      var (_, unkownIntent) = _unknownIntentMatcher.TryGetIntent(req.Request);
      return block.Handle(unkownIntent as UnknownIntent);
    }

    void Log<T>(T t) => Console.WriteLine(t);

    (bool ok, HandleResult result) HandleGenericIntent<TIntent>(object block, TIntent intent)
      where TIntent : IntentBase => block is IIntentHandler<TIntent> handler ? (true, handler.Handle(intent)) : default;

    (bool ok, HandleResult result) HandleIntent(object block, IntentBase intent) =>
      ((bool ok, HandleResult result)) _handleGenericIntentMethodInfo.MakeGenericMethod(intent.GetType())
        .Invoke(this, new[] {block, intent});
  }

  public class ProcessorOptions {
    public List<IIntentMatcher> CommonIntentMatchers { get; } = new List<IIntentMatcher>();
    public ResponseModel ErrorResponse { get; set; }
  }
}
