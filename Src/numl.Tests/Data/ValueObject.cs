using System.Collections.Generic;
using numl.Model;

namespace numl.Tests.Data
{
  public class ValueObject
  {
    [StringLabel]
    public string R { get; set; }

    [Feature]
    public int V1 { get; set; }

    [Feature]
    public int V2 { get; set; }

    public static IEnumerable<ValueObject> GetData()
    {
      for (var i = 0; i < 100; i++)
        yield return new ValueObject {V1 = 1, V2 = i, R = i > 50 ? "l" : "s"};
    }
  }
}