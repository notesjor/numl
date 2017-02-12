using numl.Math.LinearAlgebra;

namespace numl.Supervised.KNN
{
  /// <summary>A knn generator.</summary>
  public class KNNGenerator : Generator
  {
    /// <summary>Constructor.</summary>
    /// <param name="k">(Optional) the int to process.</param>
    public KNNGenerator(int k = 5) { K = k; }

    /// <summary>Gets or sets the k.</summary>
    /// <value>The k.</value>
    public int K { get; set; }

    /// <summary>Generate model based on a set of examples.</summary>
    /// <param name="X">The Matrix to process.</param>
    /// <param name="y">The Vector to process.</param>
    /// <returns>Model.</returns>
    public override IModel Generate(Matrix X, Vector y)
    {
      Preprocess(X);

      return new KNNModel
      {
        Descriptor = Descriptor,
        NormalizeFeatures = NormalizeFeatures,
        FeatureNormalizer = FeatureNormalizer,
        FeatureProperties = FeatureProperties,
        X = X,
        Y = y,
        K = K
      };
    }
  }
}