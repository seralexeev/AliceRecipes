namespace AliceRecipes.Models {
  public class QueryResult<T> {
    public int TotalCount { get; set; }
    public PageInfoResult PageInfo { get; set; }
    public T[] Items { get; set; }
  }
}
