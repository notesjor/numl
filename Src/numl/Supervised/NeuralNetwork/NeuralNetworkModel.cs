using System.Linq;
using numl.Math.Functions;
using numl.Math.LinearAlgebra;

namespace numl.Supervised.NeuralNetwork
{
  /// <summary>A data Model for the neural network.</summary>
  public class NeuralNetworkModel : Model, ISequenceModel
  {
    /// <summary>Gets or sets the network.</summary>
    /// <value>The network.</value>
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
      Preprocess(x);

      Network.Forward(x);

      var output = Network.Out.Select(n => n.Output).ToVector();

      return OutputFunction?.Minimize(output) ?? output.Max();
    }

    /// <summary>
    ///   Predicts the given x.
    /// </summary>
    /// <param name="x">Vector of features.</param>
    /// <returns>Vector.</returns>
    public Vector PredictSequence(Vector x)
    {
      Preprocess(x);

      Network.Forward(x);

      var output = Network.Out.Select(n => n.Output).ToVector();

      return OutputFunction != null ? OutputFunction.Compute(output) : output;
    }
  }
}