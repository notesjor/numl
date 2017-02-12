using numl.Math.LinearAlgebra;

namespace numl.Supervised
{
  /// <summary>
  ///   Implements a Sequence model.
  /// </summary>
  public interface ISequenceModel : IModel
  {
    /// <summary>
    ///   Predicts the given example.
    /// </summary>
    /// <param name="x"></param>
    /// <returns></returns>
    Vector PredictSequence(Vector x);
  }
}