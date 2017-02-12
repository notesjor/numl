﻿using numl.Math.LinearAlgebra;

namespace numl.Math.Metrics
{
  /// <summary>Interface for similarity.</summary>
  public interface ISimilarity
  {
    /// <summary>Computes.</summary>
    /// <param name="x">The Vector to process.</param>
    /// <param name="y">The Vector to process.</param>
    /// <returns>A double.</returns>
    double Compute(Vector x, Vector y);
  }
}