using System.Collections.Generic;
using System.Threading.Tasks;

namespace AliceKit.Services {
  public interface IImageService {
    Task<AliceImage> UploadImage(string url);
    Task<AliceImage[]> UploadImages(IEnumerable<string> urls);
  }
}
