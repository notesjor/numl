using System.Linq;
using numl.Math.LinearAlgebra;
using numl.Reinforcement.States;

namespace numl.Reinforcement.QLearning
{
  /// <summary>
  ///   Q-Learner generator.
  /// </summary>
  public class QLearnerGenerator : ReinforcementGenerator
  {
    /// <summary>
    ///   Initializes a new QLearnerGenerator object.
    /// </summary>
    public QLearnerGenerator()
    {
      LearningRate = 0.1;
      Lambda = 0.3;
      QValue = -0.03;
      Epsilon = 10e-6;
      MaxIterations = 100;
    }

    /// <summary>
    ///   Gets or sets the Epsilon convergence parameter used in training.
    ///   <para>This is the amount of change required before learning is considered converged. The default is: 0.0001.</para>
    /// </summary>
    public double Epsilon { get; set; }

    /// <summary>
    ///   Gets or sets the lambda (discount factor) value. A higher value will prefer long-term rewards over short-term ones.
    ///   <para>This value should be between 0 and 1.</para>
    /// </summary>
    public double Lambda { get; set; }

    /// <summary>
    ///   Gets or sets the learning rate (alpha).
    /// </summary>
    public double LearningRate { get; set; }

    /// <summary>
    ///   Gets or sets the maximum number of iterations for learning until convergence.
    /// </summary>
    public double MaxIterations { get; set; }

    /// <summary>
    ///   Gets or sets the Q-value initialization parameter for the Q-utility table.
    ///   <para>
    ///     A starting value needs to allow learning in uncertain environments - i.e. factoring in negative and positive
    ///     rewards.  The default value is: -0.03.
    ///   </para>
    /// </summary>
    public double QValue { get; set; }

    /// <summary>
    ///   Generates a <see cref="QLearnerModel" /> based on states/actions with transitions and rewards.
    /// </summary>
    /// <param name="X1">Initial State matrix.</param>
    /// <param name="y">Action label vector.</param>
    /// <param name="X2">Transition State matrix.</param>
    /// <param name="r">Reward values.</param>
    /// <returns>QLearnerModel.</returns>
    public override IReinforcementModel Generate(Matrix X1, Vector y, Matrix X2, Vector r)
    {
      Preprocess(X1, y, X2, r);

      var examples = MDPConverter.GetStates(X1, y, X2, FeatureProperties, FeatureDiscretizer);

      var states = examples.Item1;
      var actions = examples.Item2;
      var statesP = examples.Item3;

      var Q = new QTable();

      // construct Q table
      for (var i = 0; i < states.Count(); i++)
      {
        var state = states.ElementAt(i);
        var action = actions.ElementAt(i);
        var stateP = statesP.ElementAt(i);

        Q.AddOrUpdate(state, action, r[i]);

        if (!Q.ContainsKey(stateP))
          Q.AddKey(stateP);
      }

      double count = states.Select(s => s.Id).Distinct().Count();

      for (var pass = 0; pass < MaxIterations; pass++)
      {
        double change = 0;

        for (var i = 0; i < states.Count(); i++)
        {
          var state = states.ElementAt(i);
          var action = actions.ElementAt(i);
          var stateP = statesP.ElementAt(i);
          var reward = r[i];

          var q = (1.0 - LearningRate) * Q[state, action]
                  + LearningRate * (reward + Lambda * Q[stateP, Q.GetMaxAction(stateP)]);

          change += 1.0 / count * System.Math.Abs(Q[state, action] - q);

          Q[state, action] = q;
        }

        if (change <= Epsilon)
          break;
      }

      return new QLearnerModel
      {
        Descriptor = Descriptor,
        TransitionDescriptor = TransitionDescriptor,
        NormalizeFeatures = NormalizeFeatures,
        FeatureNormalizer = FeatureNormalizer,
        FeatureProperties = FeatureProperties,
        FeatureDiscretizer = FeatureDiscretizer,
        LearningRate = LearningRate,
        Lambda = Lambda,
        Q = Q
      };
    }
  }
}