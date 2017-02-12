using numl.AI;

namespace numl.Reinforcement.States
{
    /// <summary>
    /// IMDPSuccessor interface.
    /// </summary>
    public interface IMDPSuccessor : ISuccessor
    {
        /// <summary>
        /// Gets the Reward for the transition state.
        /// </summary>
        double Reward { get; }
    }
}
