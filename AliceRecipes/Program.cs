using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AliceKit.Framework;
using AliceKit.Helpers;
using AliceKit.Protocol;
using AliceKit.Services;
using AliceRecipes.Blocks;
using AliceRecipes.Config;
using AliceRecipes.Intents.Instant;
using AliceRecipes.Services;
using AliceRecipes.Services.GraphQL;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using static AliceKit.Helpers.Extensions;
using static AliceKit.Helpers.PathEx;
using static Newtonsoft.Json.JsonConvert;

namespace AliceRecipes {
  public class Alice : Controller {
    static AliceConfig _config;

    static Task Main(string[] args) {
      _config = DeserializeObject<AliceConfig>(System.IO.File.ReadAllText(AppRoot / "Config" / "config.json"));
      return CreateWebHostBuilder(args).Build().RunAsync();
    }

    public static IWebHostBuilder CreateWebHostBuilder(string[] args) => WebHost.CreateDefaultBuilder(args)
      .ConfigureServices(srv => {
        srv.AddMvc().AddJsonOptions(options => {
          options.SerializerSettings.Converters.Add(new StringEnumConverter(false));
          options.SerializerSettings.ContractResolver = new MaxLengthContractResolver();
        });

        var blocks = FindBlocks().ToList();
        foreach (var block in blocks) {
          srv.AddTransient(block);
        }

        srv.AddSingleton(new GraphQLClient(_config.GraphqlUrl));
        srv.AddSingleton<IImageService>(new ImageService(_config.SkillId, _config.OAuth));
        srv.AddSingleton<IRecipeService, GraphQLRecipeService>();
        srv.AddSingleton<IBlockFactory>(x => new BlockFactory(x, blocks));
        srv.AddSingleton(_config);
        srv.AddSingleton(x =>
          new Processor(x.GetRequiredService<IBlockFactory>(), typeof(InitialBlock).Name, new ProcessorOptions {
            CommonIntentMatchers = {
              new InterestingFactIntent(),
              new HelpIntent(),
            }
          }));
      }).Configure(app => app.UseMvc()).ConfigureLogging((hostingContext, logging) => {
        logging.AddFilter("Microsoft", LogLevel.Critical);
        logging.AddConsole();
      });

    readonly Processor _processor;
    public Alice(Processor processor) => _processor = processor;

    [HttpPost("/")]
    public AliceResponse WebHook([FromBody] AliceRequest req) => _processor.Process(req);
  }
}
