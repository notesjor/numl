using numl.Math.LinearAlgebra;

namespace numl.Math.Functions.Cost
{
  /// <summary>
  ///   Default Collaborative Filtering cost function.
  /// </summary>
  public class CofiCostFunction : CostFunction
  {
    /// <summary>
    ///   Cached Y reshaped Matrix.
    /// </summary>
    private Matrix YReformed;

    /// <summary>
    ///   Gets or sets the number of Collaborative Features.
    /// </summary>
    public int CollaborativeFeatures { get; set; }

    /// <summary>
    ///   Gets or Sets the R matrix where each cell indicates if a reference / rating exists (e.g. 1 or 0).
    /// </summary>
    public Matrix R { get; set; }

    /// <summary>
    ///   Compute the error cost of the given Theta parameter for the training and label sets
    /// </summary>
    /// <param name="theta">Learning Theta parameters</param>
    /// <returns></returns>
    public override double ComputeCost(Vector theta)
    {
      var ThetaX = theta.Slice(0, R.Rows * CollaborativeFeatures - 1).Reshape(CollaborativeFeatures, VectorType.Col);
      var ThetaY = theta.Slice(R.Rows * CollaborativeFeatures, theta.Length - 1)
                        .Reshape(CollaborativeFeatures, VectorType.Col);

      var j = 1.0 / 2.0
          * ((ThetaY * ThetaX.T).T - YReformed).Each(i => System.Math.Pow(i, 2.0)).Each((v, r, c) => v * R[r, c]).Sum();

      if (Lambda != 0)
        j = j
            + (Lambda / 2.0 * ThetaY.Each(i => System.Math.Pow(i, 2.0)).Sum()
               + Lambda / 2.0 * ThetaX.Each(i => System.Math.Pow(i, 2.0)).Sum());
      return j;
    }

    /// <summary>
    ///   Compute the error cost of the given Theta parameter for the training and label sets
    /// </summary>
    /// <param name="theta">Learning Theta parameters</param>
    /// <returns></returns>
    public override Vector ComputeGradient(Vector theta)
    {
      var ThetaX = theta.Slice(0, R.Rows * CollaborativeFeatures - 1).Reshape(CollaborativeFeatures, VectorType.Col);
      var ThetaY = theta.Slice(R.Rows * CollaborativeFeatures, theta.Length - 1)
                        .Reshape(CollaborativeFeatures, VectorType.Col);

      var A = (ThetaY * ThetaX.T).T - YReformed;
      var S = A.Each(R, (i, j) => i * j);

      var gradX = S * ThetaY + Lambda * ThetaX;
      var gradTheta = S.T * ThetaX + Lambda * ThetaY;

      return Vector.Combine(gradX.Unshape(), gradTheta.Unshape());
    }

    /// <summary>
    ///   Initialization method for performing custom actions prior to being optimized.
    /// </summary>
    public override void Initialize()
    {
      base.Initialize();

      YReformed = Y.Reshape(X.Rows, VectorType.Row);
    }
  }
}