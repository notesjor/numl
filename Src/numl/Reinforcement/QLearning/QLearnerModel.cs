using System.Linq;
using numl.Math.LinearAlgebra;
using numl.Reinforcement.States;

namespace numl.Reinforcement.QLearning
{
  /// <summary>
  ///   A Q-Learner Model.
  /// </summary>
  public class QLearnerModel : ReinforcementModel
  {
    /// <summary>
    ///   Initializes a new Q-Learner model.
    /// </summary>
    public QLearnerModel() { Q = new QTable(); }

    /// <summary>
    ///   Gets or sets the lambda (discount factor) value. A higher value will prefer long-term rewards over immediate rewards.
    ///   <para>This value should be between 0 and 1.</para>
    /// </summary>
    public double Lambda { get; set; }

    /// <summary>
    ///   Gets or sets the learning rate (alpha).
    /// </summary>
    public double LearningRate { get; set; }

    /// <summary>
    ///   Gets or sets the Q utility table.
    /// </summary>
    public QTable Q { get; set; }

    /// <summary>
    ///   Updates the Q-Learner model by reinforcing with the new state/action feedback values.
    /// </summary>
    /// <param name="x">State vector.</param>
    /// <param name="y">Action label.</param>
    /// <param name="r">Reward value.</param>
    public override void Learn(Vector x, double y, double r)
    {
      var state = Q.Keys.Last();
      var stateP = MDPConverter.GetState(x, FeatureProperties, FeatureDiscretizer);
      var action = MDPConverter.GetAction(y, state.Id, stateP.Id);

      Q.AddOrUpdate(stateP, action, r);

      Q[state, action] = (1.0 - LearningRate) * Q[state, action]
                         + LearningRate * (r + Lambda * Q[stateP, Q.GetMaxAction(stateP)]);
    }

    /// <summary>
    ///   Updates the Q-Learner model by reinforcing with the new state/action and transition state feedback values.
    /// </summary>
    /// <param name="x1">Item features, i.e. the original State.</param>
    /// <param name="y">Action label.</param>
    /// <param name="x2">Transition state value.</param>
    /// <param name="r">Reward value.</param>
    public override void Learn(Vector x1, double y, Vector x2, double r)
    {
      var state = MDPConverter.GetState(x1, FeatureProperties, FeatureDiscretizer);
      var stateP = MDPConverter.GetState(x2, FeatureProperties, FeatureDiscretizer);
      var action = MDPConverter.GetAction(y, state.Id, stateP.Id);

      if (!Q.ContainsKey(state))
        Q.AddOrUpdate(state, action, r);

      if (!Q.ContainsKey(stateP))
        Q.AddKey(stateP);

      Q[state, action] = (1.0 - LearningRate) * Q[state, action]
                         + LearningRate * (r + Lambda * Q[stateP, Q.GetMaxAction(stateP)]);
    }

    /// <summary>
    ///   Predicts the best action for the current state.
    /// </summary>
    /// <param name="y"></param>
    /// <returns></returns>
    public override double Predict(Vector y)
    {
      var state = FeatureDiscretizer.Discretize(y, FeatureProperties);

      return Q.GetMaxAction((int) state);
    }
  }
}