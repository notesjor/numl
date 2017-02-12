using numl.Math.Kernels;
using numl.Math.LinearAlgebra;

namespace numl.Supervised.Perceptron
{
  /// <summary>A kernel perceptron generator.</summary>
  public class KernelPerceptronGenerator : Generator
  {
    /// <summary>Constructor.</summary>
    /// <param name="kernel">The kernel.</param>
    public KernelPerceptronGenerator(IKernel kernel) { Kernel = kernel; }

    /// <summary>Gets or sets the kernel.</summary>
    /// <value>The kernel.</value>
    public IKernel Kernel { get; set; }

    /// <summary>Generate model based on a set of examples.</summary>
    /// <param name="X">The Matrix to process.</param>
    /// <param name="y">The Vector to process.</param>
    /// <returns>Model.</returns>
    public override IModel Generate(Matrix X, Vector y)
    {
      Preprocess(X);

      var N = y.Length;
      var a = Vector.Zeros(N);

      // compute kernel
      var K = Kernel.Compute(X);

      var n = 1;

      // hopefully enough to converge right? ;)
      // need to be smarter about storing SPD kernels...
      var found_error = true;
      while (n < 500 && found_error)
      {
        found_error = false;
        for (var i = 0; i < N; i++)
        {
          found_error = y[i] * a.Dot(K[i]) <= 0;
          if (found_error)
            a[i] += y[i];
        }

        n++;
      }

      // anything that *matters*
      // i.e. support vectors
      var indices = a.Indices(d => d != 0);

      // slice up examples to contain
      // only support vectors
      return new KernelPerceptronModel
      {
        Kernel = Kernel,
        A = a.Slice(indices),
        Y = y.Slice(indices),
        X = X.Slice(indices),
        Descriptor = Descriptor,
        NormalizeFeatures = NormalizeFeatures,
        FeatureNormalizer = FeatureNormalizer,
        FeatureProperties = FeatureProperties
      };
    }
  }
}