using System.Collections.Generic;
using System.Linq;
using numl.Math.LinearAlgebra;
using numl.Math.Metrics;

namespace numl.Math.Linkers
{
  /// <summary>An average linker.</summary>
  public class AverageLinker : ILinker
  {
    /// <summary>The metric.</summary>
    private readonly IDistance _metric;

    /// <summary>Constructor.</summary>
    /// <param name="metric">The metric.</param>
    public AverageLinker(IDistance metric) { _metric = metric; }

    /// <summary>Distances.</summary>
    /// <param name="x">The IEnumerable&lt;Vector&gt; to process.</param>
    /// <param name="y">The IEnumerable&lt;Vector&gt; to process.</param>
    /// <returns>A double.</returns>
    public double Distance(IEnumerable<Vector> x, IEnumerable<Vector> y)
    {
      double distanceSum = 0;

      var xCount = x.Count();
      var yCount = y.Count();
      for (var i = 0; i < xCount; i++)
      for (var j = i + 1; j < yCount; j++)
        distanceSum += _metric.Compute(x.ElementAt(i), y.ElementAt(j));

      return distanceSum / (xCount * yCount);
    }
  }
}