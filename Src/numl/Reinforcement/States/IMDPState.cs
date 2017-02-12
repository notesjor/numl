using numl.AI;
using numl.Math.LinearAlgebra;

namespace numl.Reinforcement.States
{
    /// <summary>
    /// IMDPState interface.
    /// </summary>
    public interface IMDPState : IState
    {
        /// <summary>
        /// Gets or sets the feature collection.
        /// </summary>
        Vector Features { get; set; }
    }
}
