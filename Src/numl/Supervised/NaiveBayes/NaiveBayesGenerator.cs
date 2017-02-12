using System;
using System.Collections.Generic;
using System.Linq;
using numl.Math.LinearAlgebra;

namespace numl.Supervised.NaiveBayes
{
  /// <summary>A naive bayes generator.</summary>
  public class NaiveBayesGenerator : Generator
  {
    private int _vertexId;

    /// <summary>Constructor.</summary>
    /// <param name="width">The width.</param>
    public NaiveBayesGenerator(int width) { Width = width; }

    /// <summary>Gets or sets the width.</summary>
    /// <value>The width.</value>
    public int Width { get; set; }

    /// <summary>Clone measure.</summary>
    /// <param name="measures">The measures.</param>
    /// <returns>A Measure[].</returns>
    private Measure[] CloneMeasure(Measure[] measures)
    {
      var m = new Measure[measures.Length];
      for (var i = 0; i < m.Length; i++)
        m[i] = measures[i].Clone();
      return m;
    }

    /// <summary>Generate model based on a set of examples.</summary>
    /// <exception cref="InvalidOperationException">Thrown when the requested operation is invalid.</exception>
    /// <param name="X">The Matrix to process.</param>
    /// <param name="y">The Vector to process.</param>
    /// <returns>Model.</returns>
    public override IModel Generate(Matrix X, Vector y)
    {
      if (Descriptor == null)
        throw new InvalidOperationException("Cannot build naive bayes model without type knowledge!");

      // create answer probabilities
      if (!Descriptor.Label.Discrete)
        throw new InvalidOperationException("Need to use regression for non-discrete labels!");

      Preprocess(X);

      // compute Y probabilities
      var statistics = GetLabelStats(y);

      var root = new Measure
      {
        Discrete = true,
        Label = Descriptor.Label.Name,
        Probabilities = statistics
      };

      // collect feature ranges
      var features = GetBaseConditionals(X);

      // compute conditional counts
      for (var i = 0; i < y.Length; i++)
      {
        var stat = statistics.First(s => s.X.Min == y[i]);
        if (stat.Conditionals == null)
          stat.Conditionals = CloneMeasure(features);

        for (var j = 0; j < X.Cols; j++)
        {
          var s = stat.Conditionals[j];
          s.Increment(X[i, j]);
        }
      }

      // normalize into probabilities
      for (var i = 0; i < statistics.Length; i++)
      {
        var cond = statistics[i];
        for (var j = 0; j < cond.Conditionals.Length; j++)
          cond.Conditionals[j].Normalize();
      }

      // label ids
      LabelIds(root);

      return new NaiveBayesModel
      {
        Descriptor = Descriptor,
        NormalizeFeatures = NormalizeFeatures,
        FeatureNormalizer = FeatureNormalizer,
        FeatureProperties = FeatureProperties,
        Root = root
      };
    }

    /// <summary>Gets base conditionals.</summary>
    /// <param name="x">The Matrix to process.</param>
    /// <returns>An array of measure.</returns>
    private Measure[] GetBaseConditionals(Matrix x)
    {
      var features = new Measure[x.Cols];
      for (var i = 0; i < features.Length; i++)
      {
        var p = Descriptor.At(i);
        var f = new Measure
        {
          Discrete = p.Discrete,
          Label = Descriptor.ColumnAt(i)
        };

        IEnumerable<Statistic> fstats;
        if (f.Discrete)
          fstats = x[i, VectorType.Col].Distinct().OrderBy(d => d)
                                       .Select(d => Statistic.Make(p.Convert(d).ToString(), d, 1));
        else
          fstats = x[i, VectorType.Col].Segment(Width)
                                       .Select(d => Statistic.Make(f.Label, d, 1));

        f.Probabilities = fstats.ToArray();
        features[i] = f;
      }

      return features;
    }

    /// <summary>Gets label statistics.</summary>
    /// <param name="y">The Vector to process.</param>
    /// <returns>An array of statistic.</returns>
    private Statistic[] GetLabelStats(Vector y)
    {
      var stats = y.Stats();
      var statistics = new Statistic[stats.Rows];
      for (var i = 0; i < statistics.Length; i++)
      {
        var yVal = stats[i, 0];
        var s = Statistic.Make(Descriptor.Label.Convert(stats[i, 0]).ToString(), yVal);
        s.Count = (int) stats[i, 1];
        s.Probability = stats[i, 2];
        statistics[i] = s;
      }
      return statistics;
    }

    private void LabelIds(Measure m)
    {
      m.Id = ++_vertexId;
      if (m.Probabilities != null)
      {
        foreach (var s in m.Probabilities)
          s.Id = ++_vertexId;
        foreach (var s in m.Probabilities)
          if (s.Conditionals != null)
            foreach (var measure in s.Conditionals)
              LabelIds(measure);
      }
    }
  }
}