using System.Collections.Generic;
using System.Linq;
using numl.Data;
using numl.Math.Functions;

namespace numl.Supervised.NeuralNetwork
{
  /// <summary>An Artifical Neuron.</summary>
  public class Neuron : IVertex
  {
    private static int _id = -1;

    protected double _DeltaL;

    public Neuron()
    {
      // assume bias node unless
      // otherwise told through
      // links
      Constrained = false;
      IsBias = false;
      Output = 1d;
      Input = 1d;
      Delta = 0d;
      Label = null;
      Out = new List<Edge>();
      In = new List<Edge>();
      Id = ++_id;
    }

    /// <summary>Default constructor.</summary>
    public Neuron(bool isBias)
    {
      // assume bias node unless
      // otherwise told through
      // links
      Constrained = false;
      IsBias = isBias;
      Output = 1d;
      Input = 1d;
      Delta = 0d;
      Label = null;
      Out = new List<Edge>();
      In = new List<Edge>();
      Id = ++_id;
    }

    /// <summary>Gets or sets the activation function.</summary>
    /// <value>The activation.</value>
    public IFunction ActivationFunction { get; set; }

    /// <summary>
    ///   Gets or sets whether weights into this Node are constrained / preserved.
    /// </summary>
    public bool Constrained { get; set; }

    /// <summary>
    ///   Gets or sets the minor delta term.
    /// </summary>
    public double delta { get; set; }

    /// <summary>Gets or sets the delta.</summary>
    /// <value>The delta.</value>
    public double Delta { get; set; }

    /// <summary>Gets or sets the Input <see cref="Neuron" /> connections.</summary>
    /// <value>The in.</value>
    public List<Edge> In { get; set; }

    /// <summary>Gets or sets the combined input value.</summary>
    /// <value>The input.</value>
    public double Input { get; set; }

    /// <summary>
    ///   Gets or sets whether this node is a bias node.
    /// </summary>
    public bool IsBias { get; set; }

    /// <summary>
    ///   Returns true if this Node is a hidden node, otherwise returns false.
    /// </summary>
    public bool IsHidden { get { return In.Count > 0 && Out.Count > 0; } }

    /// <summary>
    ///   Returns true if this Node is in the input layer, otherwise returns false.
    /// </summary>
    public bool IsInput { get { return In.Count == 0; } }

    /// <summary>
    ///   Returns true if this Node is in the output layer, otherwise returns false.
    /// </summary>
    public bool IsOutput { get { return Out.Count == 0; } }

    /// <summary>Gets or sets the label.</summary>
    /// <value>The label.</value>
    public string Label { get; set; }

    /// <summary>
    ///   Gets or sets the layer of this Node.
    /// </summary>
    public int LayerId { get; set; }

    /// <summary>
    ///   Gets or sets the Node index in the layer.
    /// </summary>
    public int NodeId { get; set; }

    /// <summary>Gets or sets the Output <see cref="Neuron" /> connections.</summary>
    /// <value>The out.</value>
    public List<Edge> Out { get; set; }

    /// <summary>Gets or sets the output value.</summary>
    /// <value>The output.</value>
    public double Output { get; set; }

    /// <summary>
    ///   Gets or sets the output function (optional).
    /// </summary>
    public IFunction OutputFunction { get; set; }

    /// <summary>Gets or sets the unique identifier.</summary>
    /// <value>The identifier.</value>
    public int Id { get; set; }

    /// <summary>Calculates and returns the error derivative (<see cref="Delta" />) of this node.</summary>
    /// <param name="t">Error derivative from the previous layer (n + 1).</param>
    /// <param name="properties">Network training properties.</param>
    /// <returns>A double.</returns>
    public virtual double Error(double t, NetworkTrainingProperties properties)
    {
      _DeltaL = Delta;

      if (Out.Count == 0)
      {
        Delta = delta = -(t - Output);
      }

      else
      {
        if (In.Count > 0 && Out.Count > 0)
        {
          var hp = ActivationFunction.Derivative(Input);
          delta = Out.Sum(e => e.Weight * t) * hp;
        }

        Delta = Out.Sum(s => s.Target.delta * Output);
      }

      if (In.Count > 0)
        for (var edge = 0; edge < In.Count; edge++)
          In[edge].Source.Error(Delta, properties);

      return Delta;
    }

    /// <summary>Calculates and returns the Node's <see cref="Output" /> value.</summary>
    /// <remarks>Input is equal to the weights multiplied by the source <see cref="Neuron" />'s Output.</remarks>
    /// <returns>A double.</returns>
    public virtual double Evaluate()
    {
      if (In.Count > 0)
        Input = In.Sum(e => e.Weight * e.Source.Evaluate());

      Output = ActivationFunction.Compute(Input);

      if (OutputFunction != null)
        Output = OutputFunction.Compute(Input);

      return Output;
    }

    /// <summary>
    ///   Gets the current identifier assigned when creating new nodes.
    /// </summary>
    /// <returns></returns>
    public static int GetId() { return _id; }

    /// <summary>
    ///   Propogates a reset event downstream through the network.
    /// </summary>
    /// <param name="properties">Network training properties.</param>
    public virtual void Reset(NetworkTrainingProperties properties) { }

    /// <summary>
    ///   Sets the starting identifier used when creating new nodes.
    ///   <para>
    ///     When rebuilding networks across sessions it is recommended that the identifier is set to the last used
    ///     identifier.
    ///   </para>
    /// </summary>
    /// <param name="startId"></param>
    public static void SetId(int startId) { _id = startId; }

    /// <summary>Returns a string that represents the current object.</summary>
    /// <returns>A string that represents the current object.</returns>
    public override string ToString() { return string.Format("{0} ({1} | {2})", Label, Input, Output); }

    /// <summary>Propagates a weight update event upstream through the network using the supplied learning rate.</summary>
    /// <param name="properties">Network training properties.</param>
    public virtual void Update(NetworkTrainingProperties properties)
    {
      for (var edge = 0; edge < In.Count; edge++)
      {
        Delta = 1.0 / properties.Examples * Delta;

        if (edge > 0)
          Delta = Delta + properties.Lambda / properties.Examples * In[edge].Weight;

        if (!Constrained)
          In[edge].Weight = In[edge].Weight - properties.LearningRate * Delta;
        In[edge].Source.Update(properties);
      }
    }
  }
}