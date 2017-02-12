using Xunit;

namespace numl.Tests.SerializationTests
{
  [Trait("Category", "Serialization")]
  public class SerializationEngineTests : BaseSerialization
  {
    [Fact]
    public void Basic_Registration_Test()
    {
      var type = typeof(Network);
      var serialzer = type.GetSerializer();
      Assert.Equal(typeof(NetworkSerializer), serialzer.GetType());
    }
  }
}