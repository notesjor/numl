using numl.Math.Functions;
using numl.Math.Functions.Cost;
using numl.Math.Functions.Regularization;
using numl.Math.LinearAlgebra;
using numl.Math.Optimization;

namespace numl.Supervised.Regression
{
  /// <summary>A logistic regression generator.</summary>
  public class LogisticRegressionGenerator : Generator
  {
    /// <summary>
    ///   Initialises a LogisticRegressionGenerator object
    /// </summary>
    public LogisticRegressionGenerator()
    {
      Lambda = 1;
      MaxIterations = 500;
      PolynomialFeatures = 0;
      LearningRate = 0.3;
      LogisticFunction = new Logistic();

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

    /// <summary>
    ///   Gets or sets the logit function to use.
    /// </summary>
    public IFunction LogisticFunction { get; set; }

    /// <summary>Gets or sets the maximum iterations used with gradient descent.</summary>
    /// <para>The default is 500</para>
    /// <value>The maximum iterations.</value>
    public int MaxIterations { get; set; }

    /// <summary>
    ///   The additional number of polynomial features to create, i.e. for polynomial regression.
    ///   <para>(A higher value may overfit training data)</para>
    /// </summary>
    public int PolynomialFeatures { get; set; }

    /// <summary>Generate Logistic Regression model based on a set of examples.</summary>
    /// <param name="X">The Matrix to process.</param>
    /// <param name="y">The Vector to process.</param>
    /// <returns>Model.</returns>
    public override IModel Generate(Matrix X, Vector y)
    {
      X = IncreaseDimensions(X, PolynomialFeatures);

      Preprocess(X);

      // guarantee 1/0 based label vector
      y = y.ToBinary(f => f == 1d, falseValue: 0d);

      // add intercept term
      X = X.Insert(Vector.Ones(X.Rows), 0, VectorType.Col, false);

      var theta = Vector.Rand(X.Cols);

      // run gradient descent
      var optimizer = new Optimizer(theta, MaxIterations, LearningRate)
      {
        CostFunction = new LogisticCostFunction
        {
          X = X,
          Y = y,
          Lambda = Lambda,
          Regularizer = new L2Regularizer(),
          LogisticFunction = LogisticFunction
        }
      };

      optimizer.Run();

      var model = new LogisticRegressionModel
      {
        Descriptor = Descriptor,
        NormalizeFeatures = NormalizeFeatures,
        FeatureNormalizer = FeatureNormalizer,
        FeatureProperties = FeatureProperties,
        Theta = optimizer.Properties.Theta,
        LogisticFunction = LogisticFunction,
        PolynomialFeatures = PolynomialFeatures
      };

      return model;
    }

    /// <summary>
    ///   Adds a specified number of polynomial features to the training set Matrix.
    /// </summary>
    /// <param name="x">Training set</param>
    /// <param name="polynomialFeatures">Number of polynomial features to add</param>
    /// <returns></returns>
    public static Matrix IncreaseDimensions(Matrix x, int polynomialFeatures)
    {
      var Xtemp = x.Copy();
      var maxCols = Xtemp.Cols;
      for (var j = 0; j < maxCols - 1; j++)
      for (var k = 0; k <= polynomialFeatures; k++)
      for (var m = 0; m <= k; m++)
      {
        var v = (Xtemp[j, VectorType.Col].ToVector() ^ (k - m)) * (Xtemp[j + 1, VectorType.Col] ^ m).ToVector();
        Xtemp = Xtemp.Insert(v, Xtemp.Cols - 1, VectorType.Col);
      }
      return Xtemp;
    }
  }
}