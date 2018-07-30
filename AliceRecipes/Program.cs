using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using AliceRecipes.Services;
using AliceRecipes.States;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Debug;
using Newtonsoft.Json.Converters;

namespace AliceRecipes {
  public class Alice : Controller {
    private readonly IServiceProvider _serviceProvider;
    static void Main(string[] args) {
      CreateWebHostBuilder(args).Build().Run();
    }

    public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
      WebHost.CreateDefaultBuilder(args)
        .ConfigureServices(srv => {
          srv.AddMvc().AddJsonOptions(options => {
            options.SerializerSettings.Converters.Add(new StringEnumConverter(false));
          });

          srv.AddHttpClient();
          srv.AddSingleton(new HttpClient());
          srv.AddSingleton<GraphQL>();
          srv.AddSingleton<RecipeInfoReply>();
          srv
            .AddTransient<InitialPhase>()
            .AddTransient<PendingSearch>()
            .AddTransient<RecipeSelecting>()
            .AddTransient<RecipeSelected>();

          srv.AddSingleton<IRecipeService, PostgresRecipeService>();

          Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
        })
        .Configure(app => app.UseMvc()).ConfigureLogging((hostingContext, logging) => {
          logging.AddFilter("Microsoft", LogLevel.Critical);
          logging.AddConsole();
        });

    public Alice(IServiceProvider serviceProvider) {
      _serviceProvider = serviceProvider;
    }

    class DbState {
      public Type Type { get; set; }
      public object State { get; set; }
    }

    static Dictionary<string, DbState> _db = new Dictionary<string, DbState>();

    [HttpPost("/")]
    public AliceResponse WebHook([FromBody] AliceRequest req) {
      var phaseType = typeof(InitialPhase);
      object state = null;

      if (_db.TryGetValue(req.Session.SessionId, out var dbState)) {
        phaseType = dbState.Type;
        state = dbState.State;
      }

      var phase = _serviceProvider.GetRequiredService(phaseType) as PhaseBase;
      if (phase is IStatefulPhase statefulPhase) {
        statefulPhase.State = state;
      }

      var result = phase.Handle(req.Request);

      var newPhaseType = phaseType;
      var newState = state;
      if (result.Transition != null) {
        newPhaseType = result.Transition.Type;
        newState = result.Transition.StateData;
      }

      _db[req.Session.SessionId] = new DbState {
        Type = newPhaseType,
        State = newState
      };

      return new AliceResponse {
        Session = req.Session,
        Version = req.Version,
        Response = result.Response
      };
    }
  }
}
