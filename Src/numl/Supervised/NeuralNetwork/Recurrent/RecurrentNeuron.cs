using numl.Math.Functions;
using numl.Math.LinearAlgebra;
using numl.Math.Probability;

namespace numl.Supervised.NeuralNetwork.Recurrent
{
  /// <summary>
  ///   An individual Gated Recurrent Neuron
  /// </summary>
  public class RecurrentNeuron : Neuron
  {
    /// <summary>
    ///   Initializes a new Recurrent Neuron.
    /// </summary>
    public RecurrentNeuron()
    {
      H = 0d;
      Rb = 0d;
      Zb = 0d;
      Zx = Sampling.GetUniform();
      Zh = Sampling.GetUniform();
      Rx = Sampling.GetUniform();
      Rh = Sampling.GetUniform();

      if (ResetGate == null)
        ResetGate = new SteepLogistic();
      if (MemoryGate == null)
        MemoryGate = new SteepLogistic();
      if (ActivationFunction == null)
        ActivationFunction = new Tanh();
    }

    /// <summary>
    ///   Vector of H state deltas from previous time steps.
    /// </summary>
    public Vector DeltaH { get; set; }

    /// <summary>
    ///   Gets or Sets the hidden (internal) state of the neuron.
    /// </summary>
    public double H { get; set; }

    /// <summary>
    ///   Gets or sets the update (memory) gate function.
    /// </summary>
    public IFunction MemoryGate { get; set; }

    /// <summary>
    ///   Gets or sets the current Reset state value.
    /// </summary>
    public double R { get; set; }

    /// <summary>
    ///   Gets or sets the Reset gate bias value.
    /// </summary>
    public double Rb { get; set; }

    /// <summary>
    ///   Gets or sets the reset gate function.
    /// </summary>
    public IFunction ResetGate { get; set; }

    /// <summary>
    ///   Gets or sets the Reset gate to Hidden state weight value.
    ///   <para>This is equivalent to the Ur weight value.</para>
    /// </summary>
    public double Rh { get; set; }

    /// <summary>
    ///   Gets or sets the Reset gate to Input weight value.
    ///   <para>This is equivalent to the Wr value.</para>
    /// </summary>
    public double Rx { get; set; }

    /// <summary>
    ///   Gets or sets the current Update state value.
    /// </summary>
    public double Z { get; set; }

    /// <summary>
    ///   Gets or sets the Update gate bias value.
    /// </summary>
    public double Zb { get; set; }

    /// <summary>
    ///   Gets or sets the Update gate to Hidden weight value.
    ///   <para>This is equivalent to the Uz weight value.</para>
    /// </summary>
    public double Zh { get; set; }

    /// <summary>
    ///   Gets or sets the Update gate to Input weight value.
    ///   <para>This is equivalent to the Wz value.</para>
    /// </summary>
    public double Zx { get; set; }

    /// <summary>
    ///   Returns the error given the supplied error derivative.
    /// </summary>
    /// <param name="t">The error from the next layer.</param>
    /// <param name="properties">Network training properties object.</param>
    /// <returns></returns>
    public override double Error(double t, NetworkTrainingProperties properties)
    {
      //TODO: Return the correct error.
      base.Error(t, properties);

      return Delta;
    }

    /// <summary>
    ///   Evaluates the state.
    /// </summary>
    /// <returns></returns>
    public override double Evaluate()
    {
      // guarantee updates to Input
      base.Evaluate();

      if (In.Count > 0)
      {
        // is hidden unit - apply memory states
        // Input is equal to combined input weights with bias values

        R = ResetGate.Compute(Rx * Input + Rh * H + Rb);
        Z = MemoryGate.Compute(Zx * Input + Zh * H + Zb);

        var htP = ActivationFunction.Compute(Input + R * H);

        H = (1.0 - Z) * H + Z * htP;

        Output = OutputFunction?.Compute(H) ?? H;
      }

      return Output;
    }

    /// <summary>
    ///   Resets the state of the current neuron.
    /// </summary>
    /// <param name="properties">Network training properties.</param>
    public override void Reset(NetworkTrainingProperties properties)
    {
      H = 0;

      DeltaH = Vector.Zeros((int) properties[nameof(GatedRecurrentGenerator.SequenceLength)]);

      base.Reset(properties);
    }

    /// <summary>
    ///   Updates the weights using the supplied (<see cref="NetworkTrainingProperties" />)
    /// </summary>
    /// <param name="properties">Network training properties.</param>
    public override void Update(NetworkTrainingProperties properties)
    {
      // TODO: Update recurrent weights.
      base.Update(properties);
    }
  }
}