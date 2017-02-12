using System;
using Xunit;

namespace numl.Tests.SerializationTests.BasicSerialization
{
  [Trait("Category", "Serialization")]
  public class SimpleJsonTests : BaseSerialization
  {
    [Fact]
    public void MatrixSerializationTest()
    {
      Matrix m = new[,]
      {
        {Math.PI, Math.PI / 2.3, Math.PI * 1.2, Math.PI, Math.PI / 2.3, Math.PI * 1.2},
        {Math.PI, Math.PI / 2.3, Math.PI * 1.2, Math.PI, Math.PI / 2.3, Math.PI * 1.2},
        {Math.PI, Math.PI / 2.3, Math.PI * 1.2, Math.PI, Math.PI / 2.3, Math.PI * 1.2},
        {Math.PI, Math.PI / 2.3, Math.PI * 1.2, Math.PI, Math.PI / 2.3, Math.PI * 1.2},
        {Math.PI, Math.PI / 2.3, Math.PI * 1.2, Math.PI, Math.PI / 2.3, Math.PI * 1.2}
      };

      using (var w = GetWriter())
      {
        w.WriteMatrix(m);
      }

      using (var reader = GetReader())
      {
        Matrix m3 = reader.ReadMatrix();
        Assert.Equal(m, m3);
      }
    }

    [Fact]
    public void VectorSerializationTest()
    {
      Vector v = new[]
      {
        Math.PI,
        Math.PI / 2.3,
        Math.PI * 1.2,
        Math.PI,
        Math.PI / 2.3,
        Math.PI * 1.2
      };

      using (var w = GetWriter())
      {
        w.WriteVector(v);
      }

      using (var reader = GetReader())
      {
        Vector v3 = reader.ReadVector();
        Assert.Equal(v, v3);
      }
    }
  }
}