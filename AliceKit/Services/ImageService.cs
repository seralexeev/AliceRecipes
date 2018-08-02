using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AliceKit.Helpers;
using Headers = System.Collections.Generic.Dictionary<string, string>;

namespace AliceKit.Services {
  public class ImageService : IImageService {
    static readonly HttpClient HttpClient = new HttpClient();
    readonly string _skillId;
    readonly string _oauthToken;
    readonly string _url;

    public ImageService(string skillId, string oauthToken) {
      _skillId = skillId ?? throw new ArgumentNullException(nameof(skillId));
      _oauthToken = oauthToken ?? throw new ArgumentNullException(nameof(oauthToken));

      _url = $"https://dialogs.yandex.net/api/v1/skills/{skillId}/images";
    }

    public Task<AliceImage> UploadImage(string url) => HttpClient.PostAsync<AliceImage>(_url, new {url}, new Headers {
      ["Authorization"] = $"OAuth {_oauthToken}"
    });

    public Task<AliceImage[]> UploadImages(IEnumerable<string> urls) => Task.WhenAll(urls.Select(UploadImage));
  }

  public class AliceImage {
    public ImageResult Image { get; set; }

    public class ImageResult {
      public string Id { get; set; }
      public string OrigUrl { get; set; }
    }
  }
}
