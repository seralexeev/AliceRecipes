using System;
using System.Collections.Generic;
using AliceRecipes.Intents;
using AliceRecipes.States;

namespace AliceRecipes.Builders {
  public class ReplyBuilder {
    readonly ResponseModel _response;
    StateTransition _transition;

    public ReplyBuilder(string reply) => _response = new ResponseModel(reply);
    public ReplyBuilder Tts(string tts) => Set(x => x.Tts = tts);
    public ReplyBuilder EndSession() => Set(x => x.EndSession = true);

    public ReplyBuilder BigImageCard(string imageId, Action<BigImageCardBuilder> factory = null) {
      var builder = new BigImageCardBuilder(imageId);
      factory?.Invoke(builder);
      return Set(x => x.Card = builder);
    }

    public ReplyBuilder ItemsListCard(Action<ItemsListCardBuilder> factory = null) {
      var builder = new ItemsListCardBuilder();
      factory(builder);
      return Set(x => x.Card = builder);
    }

    public ReplyBuilder Transition<TState>(TState state, Func<TransitionConfig<TState>, StateTransition> cfg) {
      _transition = cfg(new TransitionConfig<TState>(state, this));
      return this;
    }

    public ReplyBuilder Transition<TPhase>() {
      _transition = new StateTransition(typeof(TPhase), null);
      return this;
    }

    public ReplyBuilder Buttons(Action<ButtonsBuilder> factory) {
      var builder = new ButtonsBuilder();
      factory(builder);
      return Set(x => x.Buttons = builder.Buttons.ToArray());
    }

    public ReplyBuilder Buttons(params ButtonModel[] buttons) => Set(x => x.Buttons = buttons);

    public static implicit operator HandleResult(ReplyBuilder replyBuilder) => new HandleResult {
      Response = replyBuilder._response,
      Transition = replyBuilder._transition
    };

    private ReplyBuilder Set(Action<ResponseModel> act) {
      act(_response);
      return this;
    }
  }

  public class ButtonsBuilder {
    public List<ButtonModel> Buttons { get; } = new List<ButtonModel>();

    public ButtonsBuilder Add(string title, bool hide = true, string url = null) {
      Buttons.Add(new ButtonModel(title, hide, url));
      return this;
    }
  }
}
