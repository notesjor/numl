using System;
using numl.Data;
using numl.Math.LinearAlgebra;

namespace numl.Supervised.NaiveBayes
{
  /// <summary>A data Model for the naive bayes.</summary>
  public class NaiveBayesModel : Model
  {
    /// <summary>Gets or sets the root.</summary>
    /// <value>The root.</value>
    public Measure Root { get; set; }

    /// <summary>
    ///   The tree decision structure
    /// </summary>
    public Tree Tree { get; set; }

    /// <summary>Predicts the given o.</summary>
    /// <exception cref="InvalidOperationException">Thrown when the requested operation is invalid.</exception>
    /// <param name="y">The Vector to process.</param>
    /// <returns>An object.</returns>
    public override double Predict(Vector y)
    {
      Preprocess(y);

      if (Root == null || Descriptor == null)
        throw new InvalidOperationException("Invalid Model - Missing information");

      var lp = Vector.Zeros(Root.Probabilities.Length);
      for (var i = 0; i < Root.Probabilities.Length; i++)
      {
        var stat = Root.Probabilities[i];
        lp[i] = System.Math.Log(stat.Probability);
        for (var j = 0; j < y.Length; j++)
        {
          var conditional = stat.Conditionals[j];
          var p = conditional.GetStatisticFor(y[j]);
          // check for missing range, assign bad probability
          lp[i] += System.Math.Log(p?.Probability ?? 10e-10);
        }
      }
      var idx = lp.MaxIndex();
      return Root.Probabilities[idx].X.Min;
    }
  }
}