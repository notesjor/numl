using System.Linq;
using numl.Math.Functions;
using numl.Math.LinearAlgebra;

namespace numl.Supervised.NeuralNetwork.Encoders
{
  /// <summary>
  ///   An Autoencoder model.
  /// </summary>
  public class AutoencoderModel : Model, ISequenceModel
  {
    /// <summary>
    ///   Gets or sets the underlying neural network of the autoencoder.
    /// </summary>
    public Network Network { get; set; }

    /// <summary>
    ///   Gets or sets the output layer function (i.e. Softmax).
    /// </summary>
    public IFunction OutputFunction { get; set; }

    /// <summary>Predicts the given o.</summary>
    /// <param name="x">The Vector to process.</param>
    /// <returns>An object.</returns>
    public override double Predict(Vector x)
    {
      var output = PredictSequence(x);

      return OutputFunction?.Minimize(output) ?? output.Sum();
    }

    /// <summary>
    ///   Autoencodes the given example.
    /// </summary>
    /// <param name="x"></param>
    /// <returns></returns>
    public Vector PredictSequence(Vector x)
    {
      Preprocess(x);

      Network.Forward(x);

      var output = Network.Out.Select(n => n.Output).ToVector();

      return OutputFunction != null ? OutputFunction.Compute(output) : output;
    }
  }
}