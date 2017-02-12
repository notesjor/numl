using System;
using numl.Math.Functions;
using numl.Math.LinearAlgebra;

namespace numl.Math.Normalization
{
  /// <summary>
  ///   Logistic Feature normalizer using sigmoid function to scale features to be between 0 and 1.
  /// </summary>
  public class LogisticNormalizer : INormalizer
  {
    /// <summary>
    ///   Initializes a new Logistic Feature Normalizer.
    /// </summary>
    public LogisticNormalizer() { Logistic = new Logistic(); }

    /// <summary>
    ///   Gets or sets the logistic function to use for scaling.
    /// </summary>
    public IFunction Logistic { get; set; }

    /// <summary>
    ///   Normalize a row vector using Logistic normalization.
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
        item[i] = Logistic.Compute(row[i]);
      return item;
    }
  }
}