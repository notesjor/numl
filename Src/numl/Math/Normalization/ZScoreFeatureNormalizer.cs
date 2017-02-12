using System;
using numl.Math.LinearAlgebra;

namespace numl.Math.Normalization
{
  /// <summary>
  ///   Z-Score Feature normalizer to scale features to be 0 mean centered (-1 to +1).
  /// </summary>
  public class ZScoreFeatureNormalizer : INormalizer
  {
    /// <summary>
    ///   Normalize a row vector using Z-Score normalization on the supplied feature properties.
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
      {
        item[i] = (row[i] - properties.Average[i]) / properties.StandardDeviation[i];
        item[i] = double.IsNaN(item[i]) || double.IsInfinity(item[i]) ? 0d : item[i];
      }
      return item;
    }
  }
}