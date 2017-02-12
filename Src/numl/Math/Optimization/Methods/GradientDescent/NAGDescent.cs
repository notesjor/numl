using numl.Math.LinearAlgebra;

namespace numl.Math.Optimization.Methods.GradientDescent
{
  /// <summary>
  ///   A Nesterov Accelerated Gradient Descent method.
  /// </summary>
  public class NAGDescent : OptimizationMethod
  {
    /// <summary>
    ///   Defines the Momentum to use.
    /// </summary>
    public double Momentum { get; set; }

    /// <summary>
    ///   Update and return the new Theta value.
    /// </summary>
    /// <param name="properties">Properties for the optimization routine.</param>
    /// <returns></returns>
    public override Vector UpdateTheta(OptimizerProperties properties)
    {
      var v = Momentum * properties.Theta - properties.LearningRate * properties.Gradient;
      return properties.Theta + Momentum * v - properties.LearningRate * properties.Gradient;
    }
  }
}