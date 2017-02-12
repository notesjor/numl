using System.Linq;
using numl.Math.Functions;
using numl.Math.LinearAlgebra;

namespace numl.Supervised.NeuralNetwork.Recurrent
{
  /// <summary>
  ///   Gated Recurrent Neural Network model.
  /// </summary>
  public class GatedRecurrentModel : Model, ISequenceModel
  {
    /// <summary>Gets or sets the network.</summary>
    /// <value>The network.</value>
    public Network Network { get; set; }

    /// <summary>
    ///   Gets or sets the output layer function (i.e. Softmax).
    /// </summary>
    public IFunction OutputFunction { get; set; }

    /// <summary>
    ///   Predicts the next label for the given input.
    /// </summary>
    /// <param name="x">State.</param>
    /// <returns></returns>
    public override double Predict(Vector x)
    {
      Preprocess(x);

      Network.Forward(x);
      // predict the next item
      var output = Network.Out.Select(n => n.Output).ToVector();

      return OutputFunction?.Compute(output).Max() ?? output.Max();
    }

    /// <summary>
    ///   Predicts the next sequence label for the given input.
    /// </summary>
    /// <param name="x">State.</param>
    /// <returns></returns>
    public Vector PredictSequence(Vector x)
    {
      Preprocess(x);

      Network.Forward(x);
      // predict the next sequence
      var output = Network.Out.Select(n => n.Output).ToVector();

      return OutputFunction != null ? OutputFunction.Compute(output) : output;
    }
  }
}