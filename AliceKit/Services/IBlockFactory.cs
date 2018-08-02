using AliceKit.Framework;

namespace AliceKit.Services {
  public interface IBlockFactory {
    BlockBase CreateBlock(string name, object state = null);
  }
}
