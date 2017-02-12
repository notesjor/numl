using System;
using System.Collections.Generic;
using System.Linq;
using numl.Data;
using numl.Math.LinearAlgebra;
using numl.Model;

namespace numl.Supervised.NeuralNetwork
{
  /// <summary>A network.</summary>
  public class Network : Graph
  {
    /// <summary>
    ///   Gets or sets the current loss of the network.
    /// </summary>
    public double Cost { get; set; }

    /// <summary>Gets or sets the in.</summary>
    /// <value>The in.</value>
    public Neuron[] In { get; set; }

    /// <summary>
    ///   Returns the number of layers in the network.
    /// </summary>
    public int Layers
    {
      get
      {
        return GetVertices().OfType<Neuron>()
                            .Select(s => s.LayerId).Distinct().Count();
      }
    }

    /// <summary>Gets or sets the out.</summary>
    /// <value>The out.</value>
    public Neuron[] Out { get; set; }

    /// <summary>Gets a label.</summary>
    /// <param name="n">The Node to process.</param>
    /// <param name="d">The Descriptor to process.</param>
    /// <returns>The label.</returns>
    internal static string GetLabel(int n, Descriptor d)
    {
      string label;
      try
      {
        label = Enum.GetName(d.Label.Type, n);
      }
      // TODO: fix cheesy way of getting around IsEnum
      catch
      {
        if (d.Label is StringProperty && ((StringProperty) d.Label).AsEnum)
          label = ((StringProperty) d.Label).Dictionary[n];
        else
          label = d.Label.Name;
      }

      return label;
    }

    /// <summary>
    ///   Returns the Nodes from the specified layer, where 0 is the Input layer.
    /// </summary>
    /// <param name="layer">The layer index to retrieve Nodes from.</param>
    /// <returns></returns>
    public IEnumerable<Neuron> GetNodes(int layer)
    {
      return GetVertices().OfType<Neuron>().Where(n => n.LayerId == layer).ToArray();
    }

    /// <summary>
    ///   Returns a new Network instance.
    /// </summary>
    public static Network New()
    {
      return new Network();
    }

    #region Computation

    /// <summary>Forwards the given x coordinate.</summary>
    /// <exception cref="InvalidOperationException">Thrown when the requested operation is invalid.</exception>
    /// <param name="x">The Vector to process.</param>
    public void Forward(Vector x)
    {
      if (In.Length != x.Length + 1)
        throw new InvalidOperationException("Input nodes not aligned to input vector");

      // this should forward the input through the network
      // from the input nodes through the layers to the output layer

      // set input (with bias inputs)
      for (var i = 0; i < In.Length; i++)
        In[i].Input = i == 0 ? 1.0 : x[i - 1];
      // evaluate
      for (var i = 0; i < Out.Length; i++)
        Out[i].Evaluate();
    }

    /// <summary>Backpropagates the errors through the network given the supplied label.</summary>
    /// <param name="y">Label to process.</param>
    /// <param name="properties">Network training properties for use in learning.</param>
    public void Back(double y, NetworkTrainingProperties properties)
    {
      Cost = Score.ComputeRMSE(Vector.Create(Out.Length, () => y), Out.Select(s => s.Output).ToVector());

      // propagate error gradients
      for (var i = 0; i < Out.Length; i++)
        Out[i].Error(y, properties);

      // reset weights
      for (var i = 0; i < Out.Length; i++)
        Out[i].Update(properties);
    }

    /// <summary>Backpropagates the errors through the network given the supplied sequence label.</summary>
    /// <param name="y">Output vector to process.</param>
    /// <param name="properties">Network training properties for use in learning.</param>
    /// <param name="update">Indicates whether to update the weights after computing the errors.</param>
    public void Back(Vector y, NetworkTrainingProperties properties, bool update = true)
    {
      Cost = Score.ComputeRMSE(y, Out.Select(s => s.Output).ToVector());

      // CK
      // propagate error gradients
      for (var i = 0; i < Out.Length; i++)
        Out[i].Error(y[i], properties);

      if (update)
        for (var i = 0; i < Out.Length; i++)
          Out[i].Update(properties);
    }

    /// <summary>
    ///   Resets the neurons in the entire graph (see <see cref="Neuron.Reset(NetworkTrainingProperties)" />).
    /// </summary>
    /// <param name="properties">Network training properties object.</param>
    public void ResetStates(NetworkTrainingProperties properties)
    {
      foreach (var node in GetVertices().OfType<Neuron>())
        node.Reset(properties);
    }

    #endregion

    #region Graph

    /// <summary>
    ///   Adds the Neuron to the underlying graph.
    /// </summary>
    /// <param name="node">Neuron to add.</param>
    /// <returns></returns>
    public Neuron AddNode(Neuron node)
    {
      AddVertex(node);

      return node;
    }

    /// <summary>
    ///   Removes the specified node from the network, including any connections.
    /// </summary>
    /// <param name="node">Neuron to remove from the network.</param>
    public void RemoveNode(Neuron node)
    {
      // remove this node from parent edges
      for (var i = 0; i < node.In.Count; i++)
      {
        var inode = node.In.ElementAt(i).Source;
        for (var j = inode.Out.Count - 1; j >= 0; j--)
          if (inode.Out.ElementAt(j).Target == node)
            inode.Out.RemoveAt(j);
      }

      // remove this node from child edges
      for (var j = 0; j < node.Out.Count; j++)
      {
        var onode = node.Out.ElementAt(j).Target;
        for (var i = onode.In.Count - 1; i >= 0; i--)
          if (onode.In.ElementAt(i).Source == node)
            onode.In.RemoveAt(i);
      }

      node.In?.Clear();
      node.Out?.Clear();

      RemoveVertex(node);
    }

    /// <summary>
    ///   Adds the Edge to the underlying graph.
    /// </summary>
    /// <param name="edge">Edge to add.</param>
    /// <returns></returns>
    public Edge AddEdge(Edge edge)
    {
      base.AddEdge(edge);

      return edge;
    }

    #endregion
  }
}