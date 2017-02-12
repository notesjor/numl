using System;
using System.Collections.Generic;
using System.Linq;
using numl.Math;
using numl.Math.LinearAlgebra;
using numl.Math.Normalization;
using numl.Model;
using numl.Utils;

namespace numl.Supervised
{
  /// <summary>A generator.</summary>
  public abstract class Generator : IGenerator
  {
    /// <summary>
    ///   Initializes a new Generator instance.
    /// </summary>
    protected Generator()
    {
      NormalizeFeatures = false;
      FeatureNormalizer = new MinMaxNormalizer();
    }

    /// <summary>
    ///   If <c>True</c>, examples will keep their original ordering from the set.
    /// </summary>
    public bool PreserveOrder { get; set; }

    /// <summary>Generate model based on a set of examples.</summary>
    /// <exception cref="InvalidOperationException">Thrown when the requested operation is invalid.</exception>
    /// <param name="description">The description.</param>
    /// <param name="examples">Example set.</param>
    /// <returns>Model.</returns>
    public IModel Generate(Descriptor description, IEnumerable<object> examples)
    {
      if (!examples.Any())
        throw new InvalidOperationException("Empty example set.");

      Descriptor = description;
      if (Descriptor.Features == null || Descriptor.Features.Length == 0)
        throw new InvalidOperationException("Invalid descriptor: Empty feature set!");
      if (Descriptor.Label == null)
        throw new InvalidOperationException("Invalid descriptor: Empty label!");

      var doubles = Descriptor.Convert(examples.Shuffle());
      var tuple = doubles.ToExamples();

      return Generate(tuple.Item1, tuple.Item2);
    }

    /// <summary>
    ///   Generate model from descriptor and examples
    /// </summary>
    /// <typeparam name="T">Object type</typeparam>
    /// <param name="descriptor">Desriptor</param>
    /// <param name="examples">Examples</param>
    /// <returns>Model</returns>
    public IModel Generate<T>(Descriptor descriptor, IEnumerable<T> examples) where T : class
    {
      return Generate(descriptor, examples as IEnumerable<object>);
    }

    /// <summary>Generate model based on a set of examples.</summary>
    /// <param name="x">The Matrix to process.</param>
    /// <param name="y">The Vector to process.</param>
    /// <returns>Model.</returns>
    public abstract IModel Generate(Matrix x, Vector y);

    /// <summary>Gets or sets the descriptor.</summary>
    /// <value>The descriptor.</value>
    public Descriptor Descriptor { get; set; }

    /// <summary>
    ///   Gets or sets the feature normalizer to use for each item.
    /// </summary>
    public INormalizer FeatureNormalizer { get; set; }

    /// <summary>
    ///   Gets or sets the Feature properties from the original training set.
    /// </summary>
    public Summary FeatureProperties { get; set; }

    /// <summary>
    ///   Gets or sets whether to perform feature normalisation using the specified Feature Normalizer.
    /// </summary>
    public bool NormalizeFeatures { get; set; }

    /// <summary>Generate model based on a set of examples.</summary>
    /// <exception cref="InvalidOperationException">Thrown when the requested operation is invalid.</exception>
    /// <param name="examples">Example set.</param>
    /// <returns>Model.</returns>
    public IModel Generate(IEnumerable<object> examples)
    {
      if (!examples.Any())
        throw new InvalidOperationException("Empty example set.");

      if (Descriptor == null)
        throw new InvalidOperationException("Descriptor is null");

      return Generate(Descriptor, examples);
    }

    /// <summary>Event queue for all listeners interested in ModelChanged events.</summary>
    public event EventHandler<ModelEventArgs> ModelChanged;

    /// <summary>Raises the model event.</summary>
    /// <param name="sender">Source of the event.</param>
    /// <param name="e">Event information to send to registered event handlers.</param>
    protected virtual void OnModelChanged(object sender, ModelEventArgs e)
    {
      var handler = ModelChanged;
      handler?.Invoke(sender, e);
    }

    /// <summary>
    ///   Override to perform custom pre-processing steps on the raw Matrix data.
    /// </summary>
    /// <param name="X">Matrix of examples.</param>
    /// <returns></returns>
    public virtual void Preprocess(Matrix X)
    {
      FeatureProperties = new Summary
      {
        Average = X.Mean(VectorType.Row),
        StandardDeviation = X.StdDev(VectorType.Row),
        Minimum = X.Min(VectorType.Row),
        Maximum = X.Max(VectorType.Row),
        Median = X.Median(VectorType.Row)
      };

      if (NormalizeFeatures)
        if (FeatureNormalizer != null)
          for (var i = 0; i < X.Rows; i++)
          {
            var vectors = FeatureNormalizer.Normalize(X[i, VectorType.Row], FeatureProperties);
            for (var j = 0; j < X.Cols; j++)
              X[i, j] = vectors[j];
          }
    }
  }
}