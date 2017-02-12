namespace numl.Tests.Data
{
  public class FakeGuidWithError
  {
    [GuidFeature]
    public int NotAGuid { get; set; }
  }
}