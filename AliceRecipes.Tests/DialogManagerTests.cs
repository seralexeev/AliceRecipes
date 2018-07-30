//using System;
//using System.Threading.Tasks;
//using AliceRecipes.States;
//using Xunit;
//using AliceRecipes.Constants;
//using AliceRecipes.Models;
//using AliceRecipes.Services;
//
//namespace AliceRecipes.Tests {
//  class StateManager : IStateManager {
//    private readonly PhaseBase _phase;
//
//    public StateManager(PhaseBase phase) => _phase = phase;
//
//    public PhaseBase GetState(AliceRequest request) => _phase;
//  }
//
//  class Service : IRecipeService {
//    private readonly RecipePreview[] _searchResult;
//    private readonly int _total;
//
//    public Service(RecipePreview[] searchResult, int total) {
//      _searchResult = searchResult;
//      _total = total;
//    }
//
//    public Task<SearchResult<RecipePreview>> Find(string q, int limit, int offset) {
//      return Task.FromResult(new SearchResult<RecipePreview> {
//        Items = _searchResult,
//        Total = _total
//      });
//    }
//  }
//
//  public class DialogManagerTests {
//    [Fact]
//    public void InitialState_Any_Greeting() {
//      var (response, state) = Create<InitialPhase>("");
//
//      Assert.Equal("Привет", response.Response.Text);
//      Assert.IsType<PendingSearch>(state);
//    }
//
//    [Fact]
//    public void PendingSearchState_Cancel_Greeting() {
//      foreach (var word in FixedList.CancelWords) {
//        var (response, state) = Create(word);
//
//        Assert.NotNull(response);
//        Assert.Equal("Давай попробуем еще раз", response.Response.Text);
//        Assert.IsType<PendingSearch>(state);
//      }
//    }
//
//    [Fact]
//    public void PendingSearchState_SearchRequest_Found() {
//      var searchResults = Array.Empty<RecipePreview>();
//      var state = new PendingSearch(new Service(searchResults, 10));
//      var (response, newState) = Create(state, "макароны");
//
//      Assert.NotNull(response);
//      Assert.NotEmpty(response.Response.Text);
//      Assert.IsType<RecipeSelecting>(newState);
//    }
//
//    public (AliceResponse, PhaseBase) Create<TState>(string utterance) where TState : PhaseBase, new() =>
//      Create(new TState(), utterance);
//
//    public (AliceResponse, PhaseBase) Create<TState>(TState state, string utterance)
//      where TState : PhaseBase {
//      var manager = new DialogManager(new StateManager(state));
//      var request = new AliceRequest {
//        Request = new RequestModel {
//          OriginalUtterance = utterance
//        }
//      };
//
//      return manager.Handle(request);
//    }
//  }
//}


