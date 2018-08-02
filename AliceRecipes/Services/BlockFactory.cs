using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AliceKit.Framework;
using AliceKit.Services;
using Microsoft.Extensions.DependencyInjection;

namespace AliceRecipes.Services {
  public class BlockFactory : IBlockFactory {
    readonly IServiceProvider _serviceProvider;
    readonly Dictionary<string, Type> _blocks;

    public BlockFactory(IServiceProvider serviceProvider, IEnumerable<Type> blocks) {
      _serviceProvider = serviceProvider;
      _blocks = blocks.GroupBy(x => x.Name).ToDictionary(x => x.Key, x => x.FirstOrDefault());
    }

    public BlockBase CreateBlock(string name, object state = null) {
      var block = _serviceProvider.GetRequiredService(_blocks[name]) as BlockBase;
      if (block is IStatefulBlock sblock) {
        sblock.State = state;
      }

      return block;
    }
  }
}
