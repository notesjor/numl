using System;
using System.Linq;
using System.Text;
using numl.Data;
using numl.Math.LinearAlgebra;

namespace numl.Supervised.DecisionTree
{
  /// <summary>A data Model for the decision tree.</summary>
  public class DecisionTreeModel : Model
  {
    /// <summary>Default constructor.</summary>
    public DecisionTreeModel()
    {
      // no hint
      Hint = double.Epsilon;
    }

    /// <summary>Gets or sets the hint.</summary>
    /// <value>The hint.</value>
    public double Hint { get; set; }

    public Tree Tree { get; set; }

    /// <summary>Predicts the given y coordinate.</summary>
    /// <param name="y">The Vector to process.</param>
    /// <returns>A double.</returns>
    public override double Predict(Vector y)
    {
      Preprocess(y);

      return WalkNode(y, (Node) Tree.Root);
    }

    /// <summary>Print node.</summary>
    /// <param name="n">The Node to process.</param>
    /// <param name="pre">The pre.</param>
    /// <returns>A string.</returns>
    private string PrintNode(Node n, string pre)
    {
      if (n.IsLeaf)
        return string.Format("{0} +({1}, {2})\n", pre, Descriptor.Label.Convert(n.Value), n.Value);
      var sb = new StringBuilder();
      sb.AppendLine(string.Format("{0}[{1}, {2:0.0000}]", pre, n.Name, n.Gain));
      foreach (Edge edge in Tree.GetOutEdges(n))
      {
        sb.AppendLine(string.Format("{0} |- {1}", pre, edge.Label));
        sb.Append(PrintNode((Node) Tree.GetVertex(edge.ChildId), string.Format("{0} |\t", pre)));
      }

      return sb.ToString();
    }

    /// <summary>Returns a string that represents the current object.</summary>
    /// <returns>A string that represents the current object.</returns>
    public override string ToString()
    {
      return PrintNode((Node) Tree.Root, "\t");
    }

    /// <summary>Walk node.</summary>
    /// <exception cref="InvalidOperationException">Thrown when the requested operation is invalid.</exception>
    /// <param name="v">The Vector to process.</param>
    /// <param name="node">The node.</param>
    /// <returns>A double.</returns>
    private double WalkNode(Vector v, Node node)
    {
      if (node.IsLeaf)
        return node.Value;

      // Get the index of the feature for this node.
      var col = node.Column;
      if (col == -1)
        throw new InvalidOperationException("Invalid Feature encountered during node walk!");

      var edges = Tree.GetOutEdges(node).ToArray();
      for (var i = 0; i < edges.Length; i++)
      {
        var edge = (Edge) edges[i];
        if (edge.Discrete && v[col] == edge.Min)
          return WalkNode(v, (Node) Tree.GetVertex(edge.ChildId));
        if (!edge.Discrete && v[col] >= edge.Min && v[col] < edge.Max)
          return WalkNode(v, (Node) Tree.GetVertex(edge.ChildId));
      }

      if (Hint != double.Epsilon)
        return Hint;
      throw new InvalidOperationException(
        string.Format(
          "Unable to match split value {0} for feature {1}[2]\nConsider setting a Hint in order to avoid this error.",
          v[col],
          Descriptor.At(col),
          col));
    }
  }
}