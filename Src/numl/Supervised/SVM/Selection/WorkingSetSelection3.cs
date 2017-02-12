using System;
using numl.Math.LinearAlgebra;
using numl.Math.Probability;
using numl.Utils;

namespace numl.Supervised.SVM.Selection
{
  /// <summary>
  ///   Implements Working Set Selection 3 which uses second order information for selecting new pairs.
  /// </summary>
  public class WorkingSetSelection3 : ISelection
  {
    /// <summary>
    ///   Gets or sets the starting bias value (Optional, default is 0).
    /// </summary>
    public double Bias { get; set; }

    /// <summary>
    ///   Gets or sets the standard regularization value C.
    ///   <para>Lower C values will prevent overfitting.</para>
    /// </summary>
    public double C { get; set; }

    /// <summary>
    ///   Gets or sets the margin tolerance factor (default is 0.001).
    /// </summary>
    public double Epsilon { get; set; }

    /// <summary>
    ///   Gets a new working set selection of i, j pair.
    /// </summary>
    /// <param name="i">Current working set pair i.</param>
    /// <param name="j">Current working set pair j.</param>
    /// <param name="gradient">Current Gradient vector.</param>
    /// <param name="alpha">Current alpha parameter vector.</param>
    /// <returns>New working pairs of i, j.  Returns </returns>
    public Tuple<int, int> GetWorkingSet(int i, int j, Vector gradient, Vector alpha)
    {
      int m = Y.Length, ij = -1, jj = -1;
      double maxGrad = double.NegativeInfinity,
             minGrad = double.PositiveInfinity,
             minObj = double.PositiveInfinity;

      var tau = System.Math.Pow(Epsilon, 2.0);

      // choose i
      for (var k = 0; k < m; k++)
        if (Y[k] >= 1.0 && alpha[k] < C || Y[k] <= 0.0 && alpha[k] > 0.0)
        {
          var tempGrad = -Y[k] * gradient[k];
          if (!(tempGrad >= maxGrad))
            continue;
          // store new best fit
          ij = k;
          maxGrad = tempGrad;
        }
      // choose j that best optimises i
      for (var k = 0; k < m; k++)
        if (Y[k] >= 1.0 && alpha[k] > 0.0 || Y[k] <= 0.0 && alpha[k] < C)
        {
          var b = maxGrad + Y[k] * gradient[k];

          if (-Y[k] * gradient[k] <= minGrad)
            minGrad = -Y[k] * gradient[k];
          if (!(b > 0.0))
            continue;
          // compute kernel sub-pair
          var a = K[ij, ij] + K[k, k] - 2 * Y[ij] * Y[k] * K[ij, k];
          if (a <= 0)
            a = tau;

          var tempObj = -(b * b) / a;
          if (!(tempObj <= minObj))
            continue;
          // store new best fit and it's cost
          jj = k;
          minObj = tempObj;
        }
      // check tolerance of computed gradient range
      if (maxGrad - minGrad < Epsilon)
        return new Tuple<int, int>(-1, -1);
      if (jj == -1 || ij == jj)
        jj = Sampling.GetUniform(-1, m).Clip(0, m - 1);

      return new Tuple<int, int>(ij, jj);
    }

    /// <summary>
    ///   Initializes the selection function.
    /// </summary>
    /// <param name="alpha">Alpha vector</param>
    /// <param name="gradient">Gradient vector.</param>
    public void Initialize(Vector alpha, Vector gradient)
    {
      alpha.Each(d => 0, false);
      gradient.Each(d => -1, false);
    }

    /// <summary>
    ///   Gets or sets the precomputed Kernel matrix.
    /// </summary>
    public Matrix K { get; set; }

    /// <summary>
    ///   Gets or sets the training example labels in +1/-1 form.
    /// </summary>
    public Vector Y { get; set; }
  }
}