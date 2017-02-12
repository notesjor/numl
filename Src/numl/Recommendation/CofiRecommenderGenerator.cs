using System.Linq;
using numl.Math;
using numl.Math.Functions.Cost;
using numl.Math.LinearAlgebra;
using numl.Math.Normalization;
using numl.Math.Optimization;
using numl.Supervised;

namespace numl.Recommendation
{
  /// <summary>
  ///   Collaborative Filtering Recommender generator.
  /// </summary>
  public class CofiRecommenderGenerator : Generator
  {
    /// <summary>
    ///   Initialises a new Collaborative Filtering generator.
    /// </summary>
    public CofiRecommenderGenerator()
    {
      NormalizeFeatures = true;

      MaxIterations = 100;
      LearningRate = 0.1;

      FeatureNormalizer = new ZeroMeanNormalizer();
    }

    /// <summary>
    ///   Gets or sets the number of Collaborative Features to learn.
    ///   <para>Each learned feature is independently obtained of other learned features.</para>
    /// </summary>
    public int CollaborativeFeatures { get; set; }

    /// <summary>
    ///   Gets the Entity features mapping index of entity items and their corresponding row index.
    /// </summary>
    public Vector EntityFeatureMap { get; set; }

    /// <summary>
    ///   Gets or sets the regularisation term Lambda.
    /// </summary>
    public double Lambda { get; set; }

    /// <summary>
    ///   Gets or sets the learning rate (alpha).
    /// </summary>
    public double LearningRate { get; set; }

    /// <summary>
    ///   Gets or sets the maximum number of training iterations to perform when optimizing.
    /// </summary>
    public int MaxIterations { get; set; }

    /// <summary>
    ///   Gets or sets the Range of the ratings, values outside of this will be treated as not provided.
    /// </summary>
    public Range Ratings { get; set; }

    /// <summary>
    ///   Gets the Reference features mapping index of reference items and their corresponding col index.
    /// </summary>
    public Vector ReferenceFeatureMap { get; set; }

    /// <summary>
    ///   Generates a new Collaborative Filtering model.
    /// </summary>
    /// <param name="X">Training matrix values.</param>
    /// <param name="y">Vector of entity identifiers.</param>
    /// <returns></returns>
    public override IModel Generate(Matrix X, Vector y)
    {
      Preprocess(X.Copy());

      // inputs are ratings from each user (X = entities x ratings), y = entity id.
      // create rating range in case we don't have one already
      if (Ratings == null)
        Ratings = new Range(X.Min(), X.Max());

      // indicator matrix of 1's where rating was provided otherwise 0's.
      var R = X.ToBinary(f => Ratings.Test(f));

      // The mean needs to be values within rating range only.
      var mean = X.GetRows().Select(
        s =>
          s.Where(w => Ratings.Test(w)).Sum() /
          s.Count(w => Ratings.Test(w))
      ).ToVector();

      // update feature averages before preprocessing features.
      FeatureProperties.Average = mean;

      Preprocess(X);

      // where references could be user ratings and entities are movies / books, etc.
      int references = X.Cols, entities = X.Rows;

      // initialize Theta parameters
      var ThetaX = Matrix.Rand(entities, CollaborativeFeatures, -1d);
      var ThetaY = Matrix.Rand(references, CollaborativeFeatures, -1d);

      ICostFunction costFunction = new CofiCostFunction
      {
        CollaborativeFeatures = CollaborativeFeatures,
        Lambda = Lambda,
        R = R,
        Regularizer = null,
        X = ThetaX,
        Y = X.Unshape()
      };

      // we're optimising two params so combine them
      var Theta = Vector.Combine(ThetaX.Unshape(), ThetaY.Unshape());

      var optimizer = new Optimizer(Theta, MaxIterations, LearningRate)
      {
        CostFunction = costFunction
      };

      optimizer.Run();

      // extract the optimised parameter Theta
      ThetaX = optimizer.Properties.Theta.Slice(0, ThetaX.Rows * ThetaX.Cols - 1).Reshape(entities, VectorType.Row);
      ThetaY = optimizer.Properties.Theta.Slice(ThetaX.Rows * ThetaX.Cols, Theta.Length - 1)
                        .Reshape(references, VectorType.Row);

      // create reference mappings, each value is the original index.
      ReferenceFeatureMap = ReferenceFeatureMap == null ? Vector.Create(references, i => i) : ReferenceFeatureMap;
      EntityFeatureMap = EntityFeatureMap == null ? Vector.Create(entities, i => i) : EntityFeatureMap;

      return new CofiRecommenderModel
      {
        Descriptor = Descriptor,
        NormalizeFeatures = NormalizeFeatures,
        FeatureNormalizer = FeatureNormalizer,
        FeatureProperties = FeatureProperties,
        Ratings = Ratings,
        ReferenceFeatureMap = ReferenceFeatureMap,
        EntityFeatureMap = EntityFeatureMap,
        Mu = mean,
        Y = y,
        Reference = X,
        ThetaX = ThetaX,
        ThetaY = ThetaY
      };
    }
  }
}