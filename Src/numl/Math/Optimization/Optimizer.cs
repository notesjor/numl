using System.Threading.Tasks;
using numl.Math.Functions.Cost;
using numl.Math.LinearAlgebra;
using numl.Math.Optimization.Methods;
using numl.Math.Optimization.Methods.GradientDescent;

namespace numl.Math.Optimization
{
  /// <summary>
  ///   Optimizer.
  /// </summary>
  public class Optimizer
  {
    /// <summary>
    ///   Initializes a new Optimizer using the default values.
    ///   <param name="theta">Theta to optimize.</param>
    ///   <param name="maxIterations">Maximum number of iterations.</param>
    ///   <param name="learningRate">Learning Rate (alpha) (Optional).</param>
    ///   <param name="momentum">Momentum parameter for use in accelerated methods (Optional).</param>
    ///   <param name="optimizationMethod">Type of optimization method to use (Optional).</param>
    ///   <param name="optimizer">An external typed optimization method to use (Optional).</param>
    /// </summary>
    public Optimizer(
      Vector theta,
      int maxIterations,
      double learningRate = 1.0,
      double momentum = 0.9,
      OptimizationMethods optimizationMethod = OptimizationMethods.StochasticGradientDescent,
      OptimizationMethod optimizer = null)
    {
      Completed = false;
      if (optimizationMethod != OptimizationMethods.External)
        switch (optimizationMethod)
        {
          case OptimizationMethods.FastGradientDescent:
            optimizer = new FastGradientDescent {Momentum = momentum};
            break;
          case OptimizationMethods.StochasticGradientDescent:
            optimizer = new StochasticGradientDescent();
            break;
          case OptimizationMethods.NAGDescent:
            optimizer = new NAGDescent {Momentum = momentum};
            break;
        }

      OpimizationMethod = optimizer;

      Properties = new OptimizerProperties
      {
        Iteration = 0,
        MaxIterations = maxIterations,
        Cost = double.MaxValue,
        Gradient = Vector.Zeros(theta.Length),
        Theta = theta,
        LearningRate = learningRate,
        Momentum = momentum
      };
    }

    /// <summary>
    ///   Gets a value indicating whether the optimizer has finished.
    /// </summary>
    public bool Completed { get; private set; }

    /// <summary>
    ///   Gets or sets the cost function to optimize.
    /// </summary>
    public ICostFunction CostFunction { get; set; }

    /// <summary>
    ///   Gets or sets the optimization method to use.
    /// </summary>
    public IOptimizationMethod OpimizationMethod { get; set; }

    /// <summary>
    ///   Gets or sets the optimization properties used.
    /// </summary>
    public OptimizerProperties Properties { get; set; }

    /// <summary>
    ///   Runs the optimization routine for the set number of iterations.
    /// </summary>
    public void Run()
    {
      CostFunction.Initialize();

      for (var x = 0; x < Properties.MaxIterations; x++)
        if (OpimizationMethod.Update(Properties))
          Step();
        else
          break;

      Completed = true;
    }

    /// <summary>
    ///   Runs the optimization routine for the set number of iterations.
    /// </summary>
    public Task RunAsync()
    {
      return Task.Factory.StartNew(Run);
    }

    /// <summary>
    ///   Performs a single step of the optimization routine.
    /// </summary>
    public void Step()
    {
      Properties.Iteration += 1;

      var lastCost = Properties.Cost;

      Properties.Cost = OpimizationMethod.UpdateCost(CostFunction, Properties);
      Properties.CostHistory.Add(Properties.Cost);

      Properties.Gradient = OpimizationMethod.UpdateGradient(CostFunction, Properties);
      Properties.GradientHistory.Add(Properties.Gradient);

      Properties.Theta = OpimizationMethod.UpdateTheta(Properties);

      if (Properties.Iteration > 1 && Properties.Cost < lastCost)
        Properties.BestTheta = Properties.Theta;
      else
        Properties.BestTheta = Properties.Theta;
    }
  }
}