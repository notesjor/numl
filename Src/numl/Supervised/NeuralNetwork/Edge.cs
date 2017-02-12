using numl.Data;
using numl.Math.Probability;
using numl.Utils;

namespace numl.Supervised.NeuralNetwork
{
  /// <summary>An edge.</summary>
  public class Edge : IEdge
  {
    /// <summary>Default constructor.</summary>
    public Edge()
    {
      // random initialization
      // R. D. Reed and R. J. Marks II, "Neural Smithing: 
      // Supervised Learning in Feedforward Artificial 
      // Neural Networks", Mit Press, 1999. pg 57
      // selecting values from range [-a,+a] where 0.1 < a < 2
      Weight = Sampling.GetUniform(1, 20) / 10d;
      if (Sampling.GetUniform() < .5)
        Weight *= -1;
    }

    /// <summary>Gets or sets the source Node.</summary>
    /// <value>The source.</value>
    public Neuron Source { get; set; }

    /// <summary>Gets or sets the target Node.</summary>
    /// <value>The target.</value>
    public Neuron Target { get; set; }

    /// <summary>Gets or sets the weight.</summary>
    /// <value>The weight.</value>
    public double Weight { get; set; }

    /// <summary>Gets or sets the identifier of the target.</summary>
    /// <value>The identifier of the target.</value>
    public int ChildId { get; set; }

    /// <summary>Gets or sets the identifier of the source.</summary>
    /// <value>The identifier of the source.</value>
    public int ParentId { get; set; }

    /// <summary>Creates a new Edge.</summary>
    /// <param name="source">Source for the.</param>
    /// <param name="target">Target for the.</param>
    /// <param name="weight">Weight parameter to initialize with.</param>
    /// <param name="epsilon">
    ///   Seed value to use when randomly selecting a weight (ignored when <paramref name="weight" /> is supplied).
    ///   <para>The weight is then chosen from the open interval: -epsilon to +epsilon.</para>
    /// </param>
    /// <returns>An Edge.</returns>
    public static Edge Create(Neuron source, Neuron target, double weight = double.NaN, double epsilon = double.NaN)
    {
      var e = new Edge
      {
        Source = source,
        Target = target,
        ParentId = source.Id,
        ChildId = target.Id
      };
      source.Out.Add(e);
      target.In.Add(e);

      if (!double.IsNaN(weight))
        e.Weight = weight;
      else if (!double.IsNaN(epsilon))
        e.Weight = GetWeight(epsilon);

      return e;
    }

    /// <summary>
    ///   Computes the weight epsilon parameter.
    /// </summary>
    /// <param name="min">Minimum activation function value.</param>
    /// <param name="max">Maximum activation function value.</param>
    /// <param name="inputs">Number of inward connections to the current node.</param>
    /// <param name="outputs">Number of outward connections from the current node.</param>
    /// <returns></returns>
    public static double GetEpsilon(double min, double max, double inputs, double outputs)
    {
      var activator = max * 4 - min * 4;
      return (activator / 2.0).Clip(1, 4) * System.Math.Sqrt(6.0 / (inputs + outputs + 1));
    }

    /// <summary>
    ///   Computes a new weight from the interval: -epsilon to +epsilon.
    /// </summary>
    /// <param name="epsilon">Precomputed epsilon for computing a </param>
    /// <returns></returns>
    public static double GetWeight(double epsilon) { return Sampling.GetUniform(-epsilon, epsilon); }

    /// <summary>Returns a string that represents the current object.</summary>
    /// <returns>A string that represents the current object.</returns>
    public override string ToString() =>
      $"{Source} ---- {Weight} ----> {Target}";
  }
}