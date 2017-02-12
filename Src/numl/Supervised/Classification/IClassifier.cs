using numl.Math.LinearAlgebra;

namespace numl.Supervised.Classification
{
  /// <summary>
  ///   Interface for Classifiers
  /// </summary>
  public interface IClassifier
  {
    /// <summary>
    ///   Calculates the raw probability of a positive value from the underlying learned Classifier model.
    /// </summary>
    /// <param name="x">A single test record.</param>
    /// <returns>Confidence value between 0 and 1.</returns>
    double PredictRaw(Vector x);
  }
}