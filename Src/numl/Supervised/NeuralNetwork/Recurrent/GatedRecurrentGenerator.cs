using System;
using System.Linq;
using numl.Math.Functions;
using numl.Math.LinearAlgebra;
using numl.Utils;

namespace numl.Supervised.NeuralNetwork.Recurrent
{
  /// <summary>
  ///   A Gated Recurrent Unit neural network generator.
  /// </summary>
  public class GatedRecurrentGenerator : Generator, ISequenceGenerator
  {
    /// <summary>
    ///   Initializes a new instance of a Gated Recurrent Network generator.
    /// </summary>
    public GatedRecurrentGenerator()
    {
      SequenceLength = 1;
      Epsilon = double.NaN;
      LearningRate = 0.1;
      Lambda = 0.0;
      ResetGate = new SteepLogistic();
      UpdateGate = new SteepLogistic();
      Activation = new Tanh();
      PreserveOrder = true;
    }

    /// <summary>Gets or sets the activation.</summary>
    /// <value>The activation.</value>
    public IFunction Activation { get; set; }

    /// <summary>
    ///   Gets or sets the weight initialization value.
    ///   <para>Weights will be randomly initialized between the range: -Epsilon and +Epsilon</para>
    /// </summary>
    public double Epsilon { get; set; }

    /// <summary>
    ///   Gets or sets the Lambda or weight decay term.
    /// </summary>
    public double Lambda { get; set; }

    /// <summary>Gets or sets the learning rate.</summary>
    /// <value>The learning rate.</value>
    public double LearningRate { get; set; }

    /// <summary>Gets or sets the maximum iterations.</summary>
    /// <value>The maximum iterations.</value>
    public int MaxIterations { get; set; }

    /// <summary>
    ///   Gets or sets the output layer function (i.e. Softmax).
    /// </summary>
    public IFunction OutputFunction { get; set; }

    /// <summary>
    ///   Gets or sets the Reset gating function used in the individual neurons.
    /// </summary>
    public IFunction ResetGate { get; set; }

    /// <summary>
    ///   Gets or sets the size of the memory timeframe.
    ///   <para>
    ///     A larger value will allow the network to learn greater long-term dependencies.  This value should be less than
    ///     the size of the training set.
    ///   </para>
    /// </summary>
    public int SequenceLength { get; set; }

    /// <summary>
    ///   Gets or sets the Update gating function used in the individual neurons.
    /// </summary>
    public IFunction UpdateGate { get; set; }

    /// <summary>
    ///   Generates a GRU neural network model for predicting sequences.
    /// </summary>
    /// <param name="X">Matrix of training data.</param>
    /// <param name="Y">Matrix of matching sequence labels.</param>
    /// <returns>GatedRecurrentModel.</returns>
    public ISequenceModel Generate(Matrix X, Matrix Y)
    {
      Preprocess(X);

      // because Seth said so...
      if (MaxIterations <= 0)
        MaxIterations = 500;

      var network = Network.New().Create(
        X.Cols,
        Y.Cols,
        Activation,
        OutputFunction,
        (i, j) => new RecurrentNeuron
        {
          ActivationFunction = Activation,
          ResetGate = ResetGate,
          MemoryGate = UpdateGate,
          DeltaH = Vector.Zeros(SequenceLength)
        },
        epsilon: Epsilon);

      var model = new GatedRecurrentModel
      {
        Descriptor = Descriptor,
        NormalizeFeatures = NormalizeFeatures,
        FeatureNormalizer = FeatureNormalizer,
        FeatureProperties = FeatureProperties,
        Network = network,
        OutputFunction = OutputFunction
      };

      var m = X.Rows;

      OnModelChanged(this, ModelEventArgs.Make(model, "Initialized"));

      var properties = NetworkTrainingProperties.Create(
        network,
        X.Rows,
        X.Cols,
        LearningRate,
        Lambda,
        MaxIterations,
        new {SequenceLength});

      var loss = Vector.Zeros(MaxIterations);

      var tuples = X.GetRows().Select((s, si) => new Tuple<Vector, Vector>(s, Y[si]));

      for (var pass = 0; pass < MaxIterations; pass++)
      {
        properties.Iteration = pass;

        tuples.Batch(
          SequenceLength,
          (idx, items) =>
          {
            network.ResetStates(properties);

            for (var i = 0; idx < items.Count(); idx++)
            {
              network.Forward(items.ElementAt(i).Item1);
              network.Back(items.ElementAt(i).Item2, properties);
            }
          },
          false);

        loss[pass] = network.Cost;

        var output = string.Format("Run ({0}/{1}): {2}", pass, MaxIterations, network.Cost);
        OnModelChanged(this, ModelEventArgs.Make(model, output));
      }

      return model;
    }

    /// <summary>
    ///   Generates a GRU neural network from the training set.
    /// </summary>
    /// <param name="X">The Matrix of example data.</param>
    /// <param name="y">The vector of example labels.</param>
    /// <returns></returns>
    public override IModel Generate(Matrix X, Vector y)
    {
      return Generate(X, y.ToMatrix(VectorType.Col));
    }
  }
}