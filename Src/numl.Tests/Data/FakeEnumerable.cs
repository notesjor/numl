using System.Collections.Generic;

namespace numl.Tests.Data
{
  public class FakeEnumerable
  {
    [EnumerableFeature(20)]
    public IEnumerable<int> Numbers1 { get; set; }

    [EnumerableFeature(5)]
    public double[] Numbers2 { get; set; }

    [EnumerableFeature(46)]
    public List<char> Numbers3 { get; set; }
  }
}