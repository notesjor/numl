using numl.AI;

namespace numl.Reinforcement.States
{
  /// <summary>
  ///   MDPSuccessorState class.
  /// </summary>
  public class MDPSuccessorState : IMDPSuccessor
  {
    /// <summary>
    ///   Initializes a new MDPSuccessorState.
    /// </summary>
    /// <param name="action">Action to the state.</param>
    /// <param name="cost">Cost of the transition state.</param>
    /// <param name="state">Transition state.</param>
    /// <param name="reward">Reward value.</param>
    public MDPSuccessorState(IAction action, double cost, IMDPState state, double reward)
    {
      Action = action;
      Cost = cost;
      State = state;
      Reward = reward;
    }

    /// <summary>
    ///   Gets or sets the reward for the successor state.
    /// </summary>
    public double Reward { get; set; }

    /// <summary>
    ///   Gets or sets the action to the next state.
    /// </summary>
    public IAction Action { get; set; }

    /// <summary>
    ///   Gets or sets the cost associated with the action.
    /// </summary>
    public double Cost { get; set; }

    /// <summary>
    ///   Gets or sets the successor MDP State.
    /// </summary>
    public IState State { get; set; }

    /// <summary>
    ///   Returns True if the supplied object equals the current object.
    /// </summary>
    /// <param name="obj">Object to test.</param>
    /// <returns>Boolean.</returns>
    public override bool Equals(object obj) { return GetHashCode() == ((IMDPSuccessor) obj).GetHashCode(); }

    /// <summary>
    ///   Returns the hash code for the current object.
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode() { return int.Parse($"{State.Id}{Action.ChildId}"); }
  }
}