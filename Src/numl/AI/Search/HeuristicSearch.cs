using numl.AI.Collections;
using numl.AI.Functions;

namespace numl.AI.Search
{
  /// <summary>
  ///   Class HeuristicSearch.
  /// </summary>
  public abstract class HeuristicSearch : ISearchStrategy
  {
    private readonly PriorityQueue<double, Node> _queue;

    /// <summary>
    ///   Initializes a new instance of the <see cref="HeuristicSearch" /> class.
    /// </summary>
    protected HeuristicSearch()
    {
      _queue = new PriorityQueue<double, Node>();
      Heuristic = new Heuristic();
    }

    /// <summary>
    ///   Gets or sets the heuristic function.
    /// </summary>
    public IHeuristicFunction Heuristic { get; set; }

    /// <summary>
    ///   Adds the specified node.
    /// </summary>
    /// <param name="node">The node.</param>
    public abstract void Add(Node node);

    /// <summary>
    ///   Counts this instance.
    /// </summary>
    /// <returns>System.Int32.</returns>
    public int Count() { return _queue.Count; }

    /// <summary>
    ///   Removes this instance.
    /// </summary>
    /// <returns>Node.</returns>
    public Node Remove() { return _queue.Dequeue(); }

    /// <summary>
    ///   Adds the specified node.
    /// </summary>
    /// <param name="node">The node.</param>
    /// <param name="cost">The cost.</param>
    public void Add(Node node, double cost) { _queue.Enqueue(cost, node); }
  }
}