using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using AliceKit;
using AliceKit.Framework;
using AliceKit.Helpers;
using AliceKit.Protocol;
using AliceRecipes.Services;
using AliceRecipes.States;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace AliceRecipes {
  class BlockFactory : IBlockFactory {
    private readonly IServiceProvider _serviceProvider;
    private Dictionary<string, Type> _blocks;

    public BlockFactory(IServiceProvider serviceProvider) {
      _serviceProvider = serviceProvider;

      _blocks = new[] {
        typeof(InitialBlock), typeof(PendingSearchBlock), typeof(RecipeSelectedBlock), typeof(RecipeSelectingBlock)
      }.GroupBy(x => x.Name).ToDictionary(x => x.Key, x => x.FirstOrDefault());
    }

    public BlockBase CreateBlock(string name, object state = null) {
      var block = _serviceProvider.GetService(_blocks[name]) as BlockBase;
      if (block is IStatefulBlock sblock) {
        sblock.State = state;
      }

      return block;
    }
  }

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
            options.SerializerSettings.ContractResolver = new MaxLengthContractResolver();
          });

          srv.AddHttpClient();
          srv.AddSingleton(new HttpClient());
          srv.AddSingleton<GraphQL>();
          srv.AddSingleton<RecipeInfoReply>();
          srv
            .AddTransient<InitialBlock>()
            .AddTransient<PendingSearchBlock>()
            .AddTransient<RecipeSelectingBlock>()
            .AddTransient<RecipeSelectedBlock>();

          srv.AddSingleton<IRecipeService, PostgresRecipeService>();

          Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
        })
        .Configure(app => app.UseMvc()).ConfigureLogging((hostingContext, logging) => {
          logging.AddFilter("Microsoft", LogLevel.Critical);
          logging.AddConsole();
        });

    public Alice(IServiceProvider serviceProvider) {
      _processor = new Processor(new BlockFactory(serviceProvider), typeof(InitialBlock).Name);
    }

    private Processor _processor;

    [HttpPost("/")]
    public AliceResponse WebHook([FromBody] AliceRequest req) => _processor.Process(req);
  }

  public class StringConvertor : JsonConverter<string> {
    private readonly int _maxLength;

    public StringConvertor(int maxLength) => _maxLength = maxLength;

    public override void WriteJson(JsonWriter writer, string value, JsonSerializer serializer) =>
      writer.WriteValue(value.TruncateBySentence(_maxLength));

    public override string ReadJson(JsonReader reader, Type objectType, string existingValue, bool hasExistingValue,
      JsonSerializer serializer) => throw new NotImplementedException();
  }

  public class MaxLengthContractResolver : CamelCasePropertyNamesContractResolver {
    protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization) {
      var property = base.CreateProperty(member, memberSerialization);
      var attr = member.GetCustomAttribute<MaxLengthAttribute>();
      if (attr != null) {
        property.Converter = new StringConvertor(attr.MaxLength);
      }

      return property;
    }
  }
}
