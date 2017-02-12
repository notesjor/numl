using System;
using System.Linq;
using numl.Math.Functions;

namespace numl.Supervised.NeuralNetwork
{
  /// <summary>
  ///   An independent Neural Network layer for use in constructing deep neural networks with ensembles, etc.
  /// </summary>
  public class NetworkLayer : Network
  {
    /// <summary>
    ///   Initializes a new Network Layer.  A network layer is sub neural network made up of inputs, hidden layer(s) and an
    ///   output layer.
    /// </summary>
    /// <param name="inputNodes">Number of Nodes at the input layer (includes bias Node).</param>
    /// <param name="outputNodes">Number of Nodes at the output layer.</param>
    /// <param name="isFullyConnected">
    ///   Indicates whether the output layer of this network is fully connected with the next
    ///   Network Layer / Network.
    /// </param>
    /// <param name="hiddenLayers">
    ///   Number of Nodes in each layer and the number of layers, where the array length is the number of
    ///   layers and each value is the number of Nodes in that layer.
    /// </param>
    /// <param name="fnNodeInitializer">
    ///   Function for creating a new Node at each layer (zero-based) and Node index
    ///   (zero-based).  The 0 index layer corresponds to the input layer.
    /// </param>
    /// <param name="isAutoencoder">Determines whether this Network layer is an auto-encoding layer.</param>
    /// <param name="layerConnections">
    ///   (Optional) Connection properties for this Network where the first dimension is the layer, second the Node index in
    ///   that layer,
    ///   and third the Node indexes in the next layer to pair with.
    ///   <para>For example: 1 => 2 => [2, 3] will link Node 2 in layer 1 to Nodes 2 + 3 in layer 2.</para>
    /// </param>
    /// <param name="createBiasNodes">
    ///   (Optional) Indicates whether bias nodes are automatically created (thus bypassing the
    ///   <paramref name="fnNodeInitializer" /> function).
    /// </param>
    /// <param name="linkBiasNodes">
    ///   (Optional) Indicates whether bias nodes in hidden layers are automatically linked to their respective hidden nodes.
    ///   <para>
    ///     If this is set to True, it will override any bias node connections specified in parameter
    ///     <paramref name="layerConnections" />
    ///   </para>
    /// </param>
    public NetworkLayer(
      int inputNodes,
      int outputNodes,
      bool isFullyConnected,
      int[] hiddenLayers,
      Func<int, int, Neuron> fnNodeInitializer,
      bool isAutoencoder = false,
      int[][][] layerConnections = null,
      bool createBiasNodes = true,
      bool linkBiasNodes = true)
    {
      IsFullyConnected = isFullyConnected;
      IsAutoencoder = isAutoencoder;

      if (!IsFullyConnected && (layerConnections == null || layerConnections.Length != 2 + hiddenLayers.Length))
        throw new ArgumentException(
          "Connections must be supplied when the output layer is not fully connected with the next Network Layer.",
          nameof(layerConnections));

      IFunction ident = new Ident();

      In = new Neuron[inputNodes];
      In[0] = new Neuron {Label = "B0", ActivationFunction = ident};

      // create input nodes
      for (var i = 1; i < In.Length; i++)
        In[i] = fnNodeInitializer(0, i);

      // create hidden layers
      var hiddenNodes = new Neuron[hiddenLayers.Length][];

      for (var hiddenLayer = 0; hiddenLayer < hiddenLayers.Count(); hiddenLayer++)
      {
        hiddenNodes[hiddenLayer] = new Neuron[hiddenLayers[hiddenLayer]];

        for (var h = 0; h < hiddenLayers[hiddenLayer]; h++)
          if (h == 0)
            if (createBiasNodes)
              hiddenNodes[hiddenLayer][0] = new Neuron {Label = $"B{hiddenLayer + 1}", ActivationFunction = ident};
            else
              hiddenNodes[hiddenLayer][0] = fnNodeInitializer(hiddenLayer + 1, h);
          else
            hiddenNodes[hiddenLayer][h] = fnNodeInitializer(hiddenLayer + 1, h);

        // do hidden to hidden node connections
        if (hiddenLayer > 0 && hiddenLayer != hiddenLayers.Length - 1)
        {
          var useConnection = layerConnections?[hiddenLayer + 1] != null;

          if (linkBiasNodes)
            for (var node = 1; node < hiddenNodes[hiddenLayer].Length; node++)
              Edge.Create(hiddenNodes[hiddenLayer - 1][0], hiddenNodes[hiddenLayer][node]);

          // connect nodes in previous layer with this layer
          for (var h = 0; h < hiddenLayers[hiddenLayer - 1]; h++)
          {
            if (linkBiasNodes && h == 0)
              continue;

            // check for connection properties
            if (useConnection && layerConnections[hiddenLayer + 1].Length > h
                && layerConnections[hiddenLayer + 1][h] != null)
              foreach (var connection in layerConnections[hiddenLayer + 1][h])
                Edge.Create(hiddenNodes[hiddenLayer - 1][h], hiddenNodes[hiddenLayer][connection]);
          }
        }
      }

      // creating output nodes
      Out = new Neuron[outputNodes];
      for (var i = 0; i < outputNodes; i++)
        Out[i] = fnNodeInitializer(hiddenLayers.Length + 1, i);

      // link last hidden layer with output nodes
      for (var i = 0; i < Out.Length; i++)
      for (var j = 0; j < hiddenNodes.Last().Length; j++)
        Edge.Create(hiddenNodes.Last().ElementAt(j), Out[i]);
    }

    /// <summary>
    ///   Gets or sets whether this layer is an autoencoding layer.
    /// </summary>
    public bool IsAutoencoder { get; set; }

    /// <summary>
    ///   Gets or sets whether this network is fully connected with the next Network Layer.
    /// </summary>
    public bool IsFullyConnected { get; set; }

    /// <summary>
    ///   Gets or sets the connections between each layer, including the input and output layers.
    /// </summary>
    public int[][][] LayerConnections { get; set; }
  }
}