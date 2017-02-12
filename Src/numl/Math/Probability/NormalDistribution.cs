using numl.Math.LinearAlgebra;

namespace numl.Math.Probability
{
  /// <summary>A normal distribution.</summary>
  public class NormalDistribution
  {
    /// <summary>Gets or sets the mu.</summary>
    /// <value>The mu.</value>
    public Vector Mu { get; set; }

    /// <summary>Gets or sets the sigma.</summary>
    /// <value>The sigma.</value>
    public Matrix Sigma { get; set; }

    /// <summary>Computes the given x coordinate.</summary>
    /// <param name="x">The Vector to process.</param>
    /// <returns>A double.</returns>
    public double Compute(Vector x) { return 0; }

    /// <summary>Estimates.</summary>
    /// <param name="X">The Matrix to process.</param>
    /// <param name="type">(Optional) the type.</param>
    public void Estimate(Matrix X, VectorType type = VectorType.Row)
    {
      var n = type == VectorType.Row ? X.Rows : X.Cols;
      var s = type == VectorType.Row ? X.Cols : X.Rows;
      Mu = X.Sum(type) / n;
      Sigma = Matrix.Zeros(s);

      for (var i = 0; i < n; i++)
      {
        var x = X[i, type] - Mu;
        Sigma += x.Outer(x);
      }

      Sigma *= 1d / (n - 1d);
    }
  }
}