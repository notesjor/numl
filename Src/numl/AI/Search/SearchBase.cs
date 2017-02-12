using System;

namespace numl.AI.Search
{
  /// <summary>
  ///   Class Search.
  /// </summary>
  public abstract class SearchBase<TState>
    where TState : class, IState
  {
    /// <summary>
    ///   Handles the <see cref="E:SuccessorExpanded" /> event.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The StateExpansionEventArgs instance containing the event data.</param>
    protected virtual void OnSuccessorExpanded(object sender, StateExpansionEventArgs e)
    {
      var handler = SuccessorExpanded;
      handler?.Invoke(sender, e);
    }

    /// <summary>
    ///   Occurs when [successor expanded].
    /// </summary>
    public event EventHandler<StateExpansionEventArgs> SuccessorExpanded;
  }
}