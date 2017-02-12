using System;
using numl.Math.Functions;
using numl.Math.LinearAlgebra;

namespace numl.Math.Normalization
{
  /// <summary>
  ///   Hyperbolic Tangent Feature normalizer to scale features to be between -1 and +1.
  /// </summary>
  public class TanhNormalizer : INormalizer
  {
    /// <summary>
    ///   Initializes a new Hyperbolic Tangent Feature Normalizer.
    /// </summary>
    public TanhNormalizer() { Tangent = new Tanh(); }

    /// <summary>
    ///   Gets or sets the tangent function to use for scaling.
    /// </summary>
    public IFunction Tangent { get; set; }

    /// <summary>
    ///   Normalize a row vector using the hyperbolic tangent (tanh)function.
    /// </summary>
    /// <param name="row"></param>
    /// <param name="properties"></param>
    /// <returns></returns>
    public Vector Normalize(Vector row, Summary properties)
    {
      if (row == null)
        throw new ArgumentNullException("Row was null");
      var item = new double[row.Length];
      for (var i = 0; i < row.Length; i++)
        item[i] = Tangent.Compute(row[i]);
      return item;
    }
  }
}