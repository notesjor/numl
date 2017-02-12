using System.Collections.Generic;
using System.Linq;
using numl.Math.LinearAlgebra;
using numl.Math.Metrics;

namespace numl.Math.Linkers
{
  /// <summary>A complete linker.</summary>
  public class CompleteLinker : ILinker
  {
    /// <summary>The metric.</summary>
    private readonly IDistance _metric;

    /// <summary>Constructor.</summary>
    /// <param name="metric">The metric.</param>
    public CompleteLinker(IDistance metric) { _metric = metric; }

    /// <summary>Distances.</summary>
    /// <param name="x">The IEnumerable&lt;Vector&gt; to process.</param>
    /// <param name="y">The IEnumerable&lt;Vector&gt; to process.</param>
    /// <returns>A double.</returns>
    public double Distance(IEnumerable<Vector> x, IEnumerable<Vector> y)
    {
      var maxDistance = double.MinValue;

      for (var i = 0; i < x.Count(); i++)
      for (var j = i + 1; j < y.Count(); j++)
      {
        var distance = _metric.Compute(x.ElementAt(i), y.ElementAt(j));

        if (distance > maxDistance)
          maxDistance = distance;
      }

      return maxDistance;
    }
  }
}