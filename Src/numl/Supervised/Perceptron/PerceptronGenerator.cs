using numl.Math.LinearAlgebra;

namespace numl.Supervised.Perceptron
{
  /// <summary>A perceptron generator.</summary>
  public class PerceptronGenerator : Generator
  {
    /// <summary>Default constructor.</summary>
    public PerceptronGenerator() { Normalize = true; }

    /// <summary>Constructor.</summary>
    /// <param name="normalize">true to normalize.</param>
    public PerceptronGenerator(bool normalize) { Normalize = normalize; }

    /// <summary>Gets or sets a value indicating whether the normalize.</summary>
    /// <value>true if normalize, false if not.</value>
    public bool Normalize { get; set; }

    /// <summary>Generate model based on a set of examples.</summary>
    /// <param name="X">The Matrix to process.</param>
    /// <param name="y">The Vector to process.</param>
    /// <returns>Model.</returns>
    public override IModel Generate(Matrix X, Vector y)
    {
      Preprocess(X);

      var w = Vector.Zeros(X.Cols);
      var a = Vector.Zeros(X.Cols);

      double wb = 0;
      double ab = 0;

      var n = 1;

      if (Normalize)
        X.Normalize(VectorType.Row);

      // repeat 10 times for *convergence*
      for (var i = 0; i < 10; i++)
      for (var j = 0; j < X.Rows; j++)
      {
        var x = X[j];
        var yi = y[j];

        // perceptron update
        if (yi * (w.Dot(x) + wb) <= 0)
        {
          w = w + yi * x;
          wb += yi;
          a = a + yi * x + n;
          ab += yi * n;
        }

        n += 1;
      }

      return new PerceptronModel
      {
        W = w - a / n,
        B = wb - ab / n,
        Normalized = Normalize,
        Descriptor = Descriptor,
        NormalizeFeatures = NormalizeFeatures,
        FeatureNormalizer = FeatureNormalizer,
        FeatureProperties = FeatureProperties
      };
    }
  }
}