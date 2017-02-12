using System;
using System.Collections.Generic;
using System.Linq;
using numl.Math;
using numl.Math.Discretization;
using numl.Math.LinearAlgebra;
using numl.Math.Normalization;
using numl.Model;
using numl.Supervised;
using numl.Utils;

namespace numl.Reinforcement
{
  /// <summary>
  ///   ReinforcementGenerator base class.
  /// </summary>
  public abstract class ReinforcementGenerator : IReinforcementGenerator
  {
    /// <summary>
    ///   Initializes a new ReinforcementGenerator instance.
    /// </summary>
    protected ReinforcementGenerator()
    {
      NormalizeFeatures = false;
      FeatureNormalizer = new MinMaxNormalizer();
    }

    /// <summary>
    ///   Gets or sets the feature discretizer to use for reducing each item.
    /// </summary>
    public IDiscretizer FeatureDiscretizer { get; set; }

    /// <summary>
    ///   Gets or sets the state/action descriptor.
    /// </summary>
    /// <value>The descriptor.</value>
    public Descriptor Descriptor { get; set; }

    /// <summary>
    ///   Gets or sets the feature normalizer to use for each item.
    /// </summary>
    public INormalizer FeatureNormalizer { get; set; }

    /// <summary>
    ///   Gets or sets the Feature properties from the original training set.
    /// </summary>
    public Summary FeatureProperties { get; set; }

    /// <summary>
    ///   Gets or sets whether to perform feature normalisation using the specified Feature Normalizer.
    /// </summary>
    public bool NormalizeFeatures { get; set; }

    /// <summary>
    ///   Generates a <see cref="IReinforcementModel" /> based on a set of temporal/continuous examples.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when the requested operation is invalid.</exception>
    /// <param name="description">The description.</param>
    /// <param name="examples">Example set.</param>
    /// <returns>IReinforcementModel.</returns>
    public IReinforcementModel Generate(Descriptor description, IEnumerable<object> examples)
    {
      if (!examples.Any())
        throw new InvalidOperationException("Empty example set.");

      Descriptor = description;
      if (Descriptor.Features == null || Descriptor.Features.Length == 0)
        throw new InvalidOperationException("Invalid descriptor: Empty feature set!");
      if (Descriptor.Label == null)
        throw new InvalidOperationException("Invalid descriptor: Empty label!");

      var doubles = Descriptor.Convert(examples);
      var tuple = doubles.ToExamples();

      var states = tuple.Item1.Copy();
      var actions = tuple.Item2;
      var rewards = Vector.Rand(tuple.Item2.Length);

      var rewardProp = description.Features.GetPropertyOfType<RewardAttribute>();
      if (rewardProp != null)
        for (var x = 0; x < examples.Count(); x++)
          rewards[x] = rewardProp.Convert(examples.ElementAt(x)).First();

      return Generate(states, actions, rewards);
    }

    /// <summary>
    ///   Generates an <see cref="IReinforcementModel" /> based on a set of temporal/continuous examples.
    /// </summary>
    /// <typeparam name="T">Object type.</typeparam>
    /// <param name="descriptor">The description .</param>
    /// <param name="examples">Example set..</param>
    /// <returns></returns>
    public IReinforcementModel Generate<T>(Descriptor descriptor, IEnumerable<T> examples) where T : class
    {
      return Generate(descriptor, examples as IEnumerable<object>);
    }

    /// <summary>
    ///   Generates an <see cref="IReinforcementModel" /> based on a set of examples and transitions.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when the requested operation is invalid.</exception>
    /// <param name="examples1">Example training set.</param>
    /// <param name="examples2">
    ///   Corresponding training set where each item represents a transition from
    ///   <paramref name="examples1" />.
    /// </param>
    /// <returns>IReinforcementModel.</returns>
    public IReinforcementModel Generate(IEnumerable<object> examples1, IEnumerable<object> examples2)
    {
      if (!examples1.Any())
        throw new InvalidOperationException("Empty example set.");

      if (Descriptor == null)
        throw new InvalidOperationException("Descriptor is null");

      return Generate(Descriptor, examples1, TransitionDescriptor, examples2);
    }

    /// <summary>
    ///   Generate an <see cref="IReinforcementModel" /> based on a set of examples and transitions.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when the requested operation is invalid.</exception>
    /// <param name="descriptor">The description.</param>
    /// <param name="examples1">Example training set or state/action pairs.</param>
    /// <param name="transitionDescriptor">
    ///   (Optional) Descriptor for extracting transition state/reward information from
    ///   <paramref name="examples2" /> set.
    /// </param>
    /// <param name="examples2">
    ///   (Optional) Corresponding training set where each item represents a transition state from
    ///   <paramref name="examples1" /> and a reward label.
    /// </param>
    /// <returns>IReinforcementModel.</returns>
    public IReinforcementModel Generate(
      Descriptor descriptor,
      IEnumerable<object> examples1,
      Descriptor transitionDescriptor,
      IEnumerable<object> examples2)
    {
      if (!examples1.Any())
        throw new InvalidOperationException("Empty example set.");

      var hasTransitionStates = examples2 != null && examples2.Any();

      Descriptor = descriptor;
      TransitionDescriptor = transitionDescriptor;

      if (Descriptor.Features == null || Descriptor.Features.Length == 0)
        throw new InvalidOperationException("Invalid State Descriptor: Empty feature set!");
      if (Descriptor.Label == null)
        throw new InvalidOperationException("Invalid State Descriptor: Empty label!");

      if (hasTransitionStates)
      {
        if (TransitionDescriptor?.Features == null || TransitionDescriptor.Features.Length == 0)
          throw new ArgumentNullException(
            $"Transition Descriptor was null. A transition desciptor is required for '{nameof(examples2)}'.");
        if (examples2.Count() != examples1.Count())
          throw new InvalidOperationException(
            $"Length of '{nameof(examples1)}' must match length of '{nameof(examples2)}'.");
      }

      var doubles = Descriptor.Convert(examples1);
      var tuple = doubles.ToExamples();

      var states = tuple.Item1.Copy();
      var actions = tuple.Item2;

      Matrix statesP;
      var rewards = Vector.Rand(tuple.Item2.Length);

      if (hasTransitionStates)
      {
        var doubles2 = TransitionDescriptor.Convert(examples2);
        var tuple2 = doubles2.ToExamples();

        statesP = tuple2.Item1;
        rewards = tuple2.Item2;
      }
      else
      {
        statesP = new Matrix(states.Rows, states.Cols);
        // assume temporal
        for (var i = 0; i < states.Rows - 1; i++)
          statesP[i, VectorType.Row] = states[i + 1, VectorType.Row];
      }

      return Generate(states, actions, statesP, rewards);
    }

    /// <summary>
    ///   Generates an <see cref="IReinforcementModel" /> from the state/action, transition states and reward training data.
    /// </summary>
    /// <param name="X1">Matrix of states or training example features.</param>
    /// <param name="y">Corresponding vector of actions.</param>
    /// <param name="X2">Matrix of corresponding transition states.</param>
    /// <param name="r">Reward values for each state/action pair.</param>
    /// <returns>IReinforcementModel object.</returns>
    public abstract IReinforcementModel Generate(Matrix X1, Vector y, Matrix X2, Vector r);

    /// <summary>
    ///   Gets or sets the transition/reward descriptor.
    /// </summary>
    public Descriptor TransitionDescriptor { get; set; }

    /// <summary>
    ///   Generates an <see cref="IReinforcementModel" /> from a descriptor and examples.
    /// </summary>
    /// <typeparam name="T">Object type</typeparam>
    /// <param name="descriptor">The description.</param>
    /// <param name="examples1">Example training set.</param>
    /// <param name="examples2">
    ///   (Optional) Corresponding training set where each item represents a transition from
    ///   <paramref name="examples1" />.
    /// </param>
    /// <returns>IReinforcementModel</returns>
    public IReinforcementModel Generate<T>(Descriptor descriptor, IEnumerable<T> examples1, IEnumerable<T> examples2)
      where T : class
    {
      return Generate(descriptor, examples1, examples2 as IEnumerable<object>);
    }

    /// <summary>
    ///   Generates a <see cref="IReinforcementModel" /> from the state/action and corresponding reward training data.
    ///   <para>This assumes a temporal sequence of training data where each row is continuous with the next.</para>
    /// </summary>
    /// <param name="X">Matrix of states or training example features.</param>
    /// <param name="y">Corresponding vector of actions.</param>
    /// <param name="r">Reward values for each state/action pair.</param>
    /// <returns>IReinforcementModel object.</returns>
    public virtual IReinforcementModel Generate(Matrix X, Vector y, Vector r)
    {
      // generate temporal data
      var X2 = new Matrix(X.Rows, X.Cols);

      for (var i = 0; i < X.Rows - 1; i++)
        X2[i, VectorType.Row] = X[i + 1, VectorType.Row];

      return Generate(X, y, X2, r);
    }

    /// <summary>
    ///   Event queue for all listeners interested in ModelChanged events.
    /// </summary>
    public event EventHandler<ModelEventArgs> ModelChanged;

    /// <summary>
    ///   Raises the model event.
    /// </summary>
    /// <param name="sender">Source of the event.</param>
    /// <param name="e">Event information to send to registered event handlers.</param>
    protected virtual void OnModelChanged(object sender, ModelEventArgs e)
    {
      ModelChanged?.Invoke(sender, e);
    }

    /// <summary>
    ///   Override to perform custom pre-processing steps on the raw Matrix data.
    /// </summary>
    /// <param name="X1">Matrix of initial states.</param>
    /// <param name="y">Vector of action labels.</param>
    /// <param name="X2">Matrix of transition states.</param>
    /// <param name="r">Vector of reward values.</param>
    /// <returns></returns>
    public virtual void Preprocess(Matrix X1, Vector y, Matrix X2, Vector r)
    {
      FeatureProperties = Summary.Summarize(X1);

      if (NormalizeFeatures)
        if (FeatureNormalizer != null)
          for (var i = 0; i < X1.Rows; i++)
          {
            var v1 = FeatureNormalizer.Normalize(X1[i, VectorType.Row], FeatureProperties);
            X1[i, VectorType.Row] = v1;

            if (X2 != null)
            {
              var v2 = FeatureNormalizer.Normalize(X2[i, VectorType.Row], FeatureProperties);
              X2[i, VectorType.Row] = v2;
            }
          }

      if (FeatureDiscretizer == null)
      {
        var temp = Matrix.VStack(X1, X2);
        var bins = new double[temp.Cols];
        for (var x = 0; x < X1.Cols; x++)
          bins[x] = temp[x, VectorType.Col].Distinct().Count();

        FeatureDiscretizer = new BinningDiscretizer(bins.ToVector());
        FeatureDiscretizer.Initialize(X1, FeatureProperties);
      }
    }
  }
}