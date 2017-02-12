using System.Collections.Generic;
using numl.AI;
using numl.Math.LinearAlgebra;

namespace numl.Reinforcement.States
{
  /// <summary>
  ///   MDPState class.
  /// </summary>
  public class MDPState : State<MDPSuccessorState>, IMDPState
  {
    private Vector _Vector;

    /// <summary>
    ///   Initializes a new MDPState object.
    /// </summary>
    /// <param name="id">State identifier.</param>
    public MDPState(int id) : base(id)
    {
      Id = id;
      Successors = new HashSet<MDPSuccessorState>();
    }

    /// <summary>
    ///   Gets or sets the state feature vector.
    /// </summary>
    public Vector Features
    {
      get { return _Vector ?? (_Vector = new Vector(new double[] {Id})); }
      set { _Vector = value; }
    }
  }
}