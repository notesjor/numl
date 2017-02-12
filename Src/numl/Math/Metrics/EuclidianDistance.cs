using numl.Math.LinearAlgebra;

namespace numl.Math.Metrics
{
  /// <summary>An euclidian distance.</summary>
  public sealed class EuclidianDistance : IDistance
  {
    /// <summary>Computes.</summary>
    /// <param name="x">The Vector to process.</param>
    /// <param name="y">The Vector to process.</param>
    /// <returns>A double.</returns>
    public double Compute(Vector x, Vector y) { return (x - y).Norm(); }
  }
}