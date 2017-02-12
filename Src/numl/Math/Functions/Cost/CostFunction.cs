﻿using numl.Math.Functions.Regularization;
using numl.Math.LinearAlgebra;

namespace numl.Math.Functions.Cost
{
  /// <summary>
  ///   Implements a CostFunction when overridden in a derived class.
  /// </summary>
  public abstract class CostFunction : ICostFunction
  {
    /// <summary>
    ///   Computes the cost of the current theta parameters against the known Y labels.
    /// </summary>
    /// <returns></returns>
    public abstract double ComputeCost(Vector theta);

    /// <summary>
    ///   Computes the current gradient step direction towards the minima.
    /// </summary>
    /// <returns></returns>
    public abstract Vector ComputeGradient(Vector theta);

    /// <summary>
    ///   Initialization method for performing custom actions prior to being optimized.
    /// </summary>
    public virtual void Initialize() { }

    /// <summary>
    ///   Gets or sets the weight decay (lambda) parameter.
    /// </summary>
    public double Lambda { get; set; }

    /// <summary>
    ///   Gets or sets the regularization method.
    /// </summary>
    public IRegularizer Regularizer { get; set; }

    /// <summary>
    ///   Gets or sets the input matrix.
    /// </summary>
    public Matrix X { get; set; }

    /// <summary>
    ///   Gets or sets the output for each row in X.
    /// </summary>
    public Vector Y { get; set; }
  }
}