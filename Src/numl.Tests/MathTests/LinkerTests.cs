using Xunit;

namespace numl.Tests.MathTests
{
  [Trait("Category", "Math")]
  public class LinkerTests
  {
    [Fact]
    public void Average_Linker_Test()
    {
      // TODO: Finish linker tests
      var a = new[] {1.0, 1.0};
      var b = new[] {1.5, 1.5};
      var c = new[] {5.0, 5.0};
      var d = new[] {3.0, 4.0};
      var e = new[] {4.0, 4.0};
      var f = new[] {3.0, 3.5};

      AverageLinker linker = new AverageLinker(new EuclidianDistance());
    }
  }
}