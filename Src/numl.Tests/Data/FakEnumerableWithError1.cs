namespace numl.Tests.Data
{
  public class FakEnumerableWithError1
  {
    [EnumerableFeature(12)]
    public int NotAnEnumerable { get; set; }
  }
}