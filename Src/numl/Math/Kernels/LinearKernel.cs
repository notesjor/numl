using numl.Math.LinearAlgebra;

namespace numl.Math.Kernels
{
  /// <summary>
  ///   The Linear Kernel measures similarity between the inner product space.
  ///   ||xi * xj||
  /// </summary>
  public class LinearKernel : IKernel
  {
    /// <summary>Computes a linear Kernel in the dimension space.</summary>
    /// <param name="m">Input Matrix.</param>
    /// <returns>Linear Kernel Matrix.</returns>
    public Matrix Compute(Matrix m)
    {
      var K = Matrix.Zeros(m.Rows);

      for (var i = 0; i < m.Rows; i++)
      for (var j = i; j < m.Rows; j++)
      {
        var xy = m[i].Dot(m[j]);
        K[i, j] = K[j, i] = xy;
      }

      return K;
    }

    /// <summary>
    ///   Computes the linear kernel function between the two input vectors.
    /// </summary>
    /// <param name="v1">Vector one.</param>
    /// <param name="v2">Vector two.</param>
    /// <returns>Similarity.</returns>
    public double Compute(Vector v1, Vector v2) { return v1.Dot(v2); }

    /// <summary>
    ///   Returns True (always) indicating this is a linear kernel.
    /// </summary>
    public bool IsLinear { get { return true; } }

    /// <summary>Projects vector into linear kernel space.</summary>
    /// <param name="m">Kernel Matrix.</param>
    /// <param name="x">Vector in original space.</param>
    /// <returns>Vector in linear kernel space.</returns>
    public Vector Project(Matrix m, Vector x)
    {
      var K = Vector.Zeros(m.Rows);

      for (var i = 0; i < K.Length; i++)
      {
        var xy = m[i].Dot(x);
        K[i] = xy;
      }

      return K;
    }
  }
}