using System.Collections.Generic;

namespace numl.AI.Search
{
  /// <summary>
  ///   Class SimpleSearch.
  /// </summary>
  public class SimpleSearch<TState, TSuccessor> : SearchBase<TState>
    where TState : class, IState
    where TSuccessor : class, ISuccessor
  {
    private readonly List<IState> _closed;

    /// <summary>
    ///   Initializes a new instance of the <see cref="SimpleSearch&lt;TState, TSuccessor&gt;" /> class.
    /// </summary>
    /// <param name="strategy">The strategy.</param>
    /// <param name="avoidRepetition">if set to <c>true</c> [avoid repetition].</param>
    public SimpleSearch(ISearchStrategy strategy, bool avoidRepetition = true)
    {
      Strategy = strategy;
      _closed = avoidRepetition ? new List<IState>() : null;
    }

    /// <summary>
    ///   Gets or sets the solution.
    /// </summary>
    /// <value>The solution.</value>
    public List<TSuccessor> Solution { get; set; }

    /// <summary>
    ///   Gets or sets the strategy.
    /// </summary>
    /// <value>The strategy.</value>
    public ISearchStrategy Strategy { get; set; }

    private void CreateSolution(Node n)
    {
      if (Solution == null)
        Solution = new List<TSuccessor>();
      var node = n;
      while (!node.IsRoot)
      {
        Solution.Add((TSuccessor) node.Successor);
        node = node.Parent;
      }
      Solution.Reverse();
    }

    /// <summary>
    ///   Finds the specified initial state.
    /// </summary>
    /// <param name="initialState">The initial state.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public virtual bool Find(TState initialState)
    {
      if (Strategy == null)
        return false;

      Strategy.Add(new Node(initialState));
      while (Strategy.Count() > 0)
      {
        var n = Strategy.Remove();
        if (n.Parent != null && n.Successor != null)
        {
          var eventArgs = new StateExpansionEventArgs(
            (TState) n.Parent.State,
            (TSuccessor) n.Successor,
            n.Cost,
            n.Depth);
          OnSuccessorExpanded(this, eventArgs);

          if (eventArgs.CancelExpansion)
            return false;
        }

        if (n.State.IsTerminal)
        {
          CreateSolution(n);
          return true;
        }

        foreach (var node in n.Expand(_closed))
        {
          Strategy.Add(node);
          _closed?.Add(node.State);
        }
      }

      return false;
    }
  }
}