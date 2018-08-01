using System;
using System.Linq;
using System.Reflection;
using AliceKit.Builders;
using AliceKit.Framework;
using AliceKit.Helpers;
using AliceKit.Intent;
using AliceKit.Protocol;
using Newtonsoft.Json;
using static AliceKit.FixedList;
using IntentSelector = System.Func<AliceKit.Protocol.RequestModel, (bool ok, AliceKit.Framework.IntentBase intent)>;

namespace AliceKit {
  public class Processor {
    readonly IBlockFactory _blockFactory;
    readonly string _defaultBlockName;
    readonly SessionManager _sessionManager = new SessionManager();

    readonly MethodInfo _handleGenericIntentMethodInfo =
      typeof(Processor).GetMethod(nameof(HandleGenericIntent), BindingFlags.Instance | BindingFlags.NonPublic);

    public Processor(IBlockFactory blockFactory, string defaultBlockName) {
      _blockFactory = blockFactory;
      _defaultBlockName = defaultBlockName;
    }

    (bool ok, HandleResult result) HandleGenericIntent<TIntent>(object block, TIntent intent)
      where TIntent : IntentBase => block is IIntentHandler<TIntent> handler ? (true, handler.Handle(intent)) : default;

    public AliceResponse Process(AliceRequest req) {
      var (ok, ctx) = _sessionManager.Get(req.Session.SessionId);

      if (!ok) {
        ctx = new ContextData {
          BlockName = _defaultBlockName,
          Session = req.Session,
          SessionId = req.Session.SessionId
        };
      }

      var result = Process(req, _blockFactory.CreateBlock(ctx.BlockName, ctx.State));

      _sessionManager.Set(new ContextData {
        BlockName = result.Transition != null ? result.Transition.Name : ctx.BlockName,
        State = result.Transition != null ? result.Transition.State : ctx.State,
        Session = ctx.Session,
        SessionId = ctx.SessionId
      });

      return new AliceResponse {
        Response = result.Response,
        Session = ctx.Session
      };
    }

    public HandleResult Process(AliceRequest req, BlockBase block) {
      try {
        var intent = new IntentSelector[] {
            new GreetingIntent().TryGetIntent,
            TryGetCancelIntent,
            TryGetNumberSelectionIntent,
            block.TryGetIntent,
            GetUnknownIntent
          }
          .Select(x => x(req.Request))
          .Where(x => x.ok)
          .Select(x => x.intent)
          .FirstOrDefault();

        Console.WriteLine($"Intent: {JsonConvert.SerializeObject(intent, Formatting.Indented)}");

        if (intent is ISelfInvokable) {
          var (okSelf, selfResult) = HandleIntent(intent, intent);
          if (okSelf) {
            return selfResult;
          }
        }

        var (ok, result) = HandleIntent(block, intent);
        if (ok) {
          return result;
        }

        var (_, unkownIntent) = GetUnknownIntent(req.Request);
        return block.Handle(unkownIntent as UnknownIntent);
      }
      catch (Exception e) {
        Console.WriteLine(e);
        return new HandleResult {
          Response = new ResponseModel("Что то пошло не так")
        };
      }
    }

    (bool ok, HandleResult result) HandleIntent(object block, IntentBase intent) =>
      ((bool ok, HandleResult result)) _handleGenericIntentMethodInfo.MakeGenericMethod(intent.GetType())
        .Invoke(this, new[] {block, intent});

    private (bool ok, IntentBase intent) TryGetCancelIntent(RequestModel req) =>
      CancelMather.Match(req.GetSanitized()) ? (true, new CancelIntent()) : default;

    private (bool ok, IntentBase intent) TryGetNumberSelectionIntent(RequestModel req) {
      var (ok, number) = NumberMather.Match(req.GetSanitized());
      return ok ? (true, new NumberSelectionIntent(number)) : default;
    }

    private (bool ok, IntentBase intent) GetUnknownIntent(RequestModel req) => (true, new UnknownIntent());
  }

  public interface IBlockFactory {
    BlockBase CreateBlock(string name, object state = null);
  }

  public interface ISelfInvokable {
  }

  public abstract class SelfInvokableIntent<TIntent> : IntentBase, ISelfInvokable, IIntentMatcher, IIntentHandler<TIntent>
    where TIntent : IntentBase {
    protected ReplyBuilder Reply(string reply) => new ReplyBuilder(reply);

    public (bool ok, IntentBase intent) TryGetIntent(RequestModel req) {
      var (ok, intent) = TryGetIntentGeneric(req);
      return ok ? (true, intent as IntentBase) : default;
    }

    public abstract (bool ok, TIntent intent) TryGetIntentGeneric(RequestModel req);
    public abstract HandleResult Handle(TIntent intent);
  }

  public class GreetingIntent : SelfInvokableIntent<GreetingIntent> {
    static readonly string[] WelcomePhrases = {
      "Привет, давно не виделись!",
      "Привет, меня зовут Василий и я днаю множество рецептов, о которых готов тебе рассказать!",
      "Привет, Давай знакомиться! Меня зовут Василий и я могу рассказть о рецептах."
    };

    public override (bool ok, GreetingIntent intent) TryGetIntentGeneric(RequestModel req) =>
      req.Contains("привет", "здарова", "здравствуй") ? (true, new GreetingIntent()) : default;

    public override HandleResult Handle(GreetingIntent intent) => Reply(WelcomePhrases.GetRandom());
  }
}
