using numl.Math.Functions.Cost;
using numl.Math.Functions.Regularization;
using numl.Math.LinearAlgebra;
using numl.Math.Optimization;

namespace numl.Supervised.Regression
{
  /// <summary>A linear regression generator.</summary>
  public class LinearRegressionGenerator : Generator
  {
    /// <summary>
    ///   Initialise a new LinearRegressionGenerator
    /// </summary>
    public LinearRegressionGenerator()
    {
      MaxIterations = 500;
      LearningRate = 0.01;

      NormalizeFeatures = true;
    }

    /// <summary>
    ///   The regularisation term Lambda
    /// </summary>
    public double Lambda { get; set; }

    /// <summary>Gets or sets the learning rate used with gradient descent.</summary>
    /// <para>The default value is 0.01</para>
    /// <value>The learning rate.</value>
    public double LearningRate { get; set; }

    /// <summary>Gets or sets the maximum iterations used with gradient descent.</summary>
    /// <para>The default is 500</para>
    /// <value>The maximum iterations.</value>
    public int MaxIterations { get; set; }

    /// <summary>Generate Linear Regression model based on a set of examples.</summary>
    /// <param name="X">The Matrix to process.</param>
    /// <param name="y">The Vector to process.</param>
    /// <returns>Model.</returns>
    public override IModel Generate(Matrix X, Vector y)
    {
      Preprocess(X);

      // copy matrix
      var copy = X.Copy();

      // add intercept term
      copy = copy.Insert(Vector.Ones(copy.Rows), 0, VectorType.Col);

      // create initial theta
      var theta = Vector.Rand(copy.Cols);

      // run gradient descent
      var optimizer = new Optimizer(theta, MaxIterations, LearningRate)
      {
        CostFunction = new LinearCostFunction
        {
          X = copy,
          Y = y,
          Lambda = Lambda,
          Regularizer = new L2Regularizer()
        }
      };

      optimizer.Run();

      // once converged create model and apply theta

      var model = new LinearRegressionModel
      {
        Descriptor = Descriptor,
        NormalizeFeatures = NormalizeFeatures,
        FeatureNormalizer = FeatureNormalizer,
        FeatureProperties = FeatureProperties,
        Theta = optimizer.Properties.Theta
      };

      return model;
    }
  }
}