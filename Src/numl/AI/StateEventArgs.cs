using System;

namespace numl.AI
{
  /// <summary>
  ///   Class StateEventArgs.
  /// </summary>
  public class StateEventArgs : EventArgs
  {
    /// <summary>
    ///   Initializes a new instance of the <see cref="StateEventArgs" /> class.
    /// </summary>
    /// <param name="state">The state.</param>
    public StateEventArgs(IState state) { State = state; }

    /// <summary>
    ///   Gets the state.
    /// </summary>
    /// <value>The state.</value>
    public IState State { get; private set; }
  }
}