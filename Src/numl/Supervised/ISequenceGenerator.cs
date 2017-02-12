using numl.Math.LinearAlgebra;

namespace numl.Supervised
{
  /// <summary>
  ///   ISequenceGenerator interface.
  /// </summary>
  public interface ISequenceGenerator
  {
    /// <summary>Generates a sequence model.</summary>
    /// <param name="X">Matrix of training data.</param>
    /// <param name="Y">Matrix of sequence labels.</param>
    /// <returns>ISequenceModel.</returns>
    ISequenceModel Generate(Matrix X, Matrix Y);
  }
}