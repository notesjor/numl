using System;
using System.Collections.Generic;
using System.Linq;

namespace numl.Math.LinearAlgebra
{
  /// <summary>An evd.</summary>
  public class Evd
  {
    /// <summary>The Matrix to process.</summary>
    private readonly Matrix A;

    /// <summary>The Matrix to process.</summary>
    private Matrix V;

    /// <summary>Constructor.</summary>
    /// <param name="a">The int to process.</param>
    public Evd(Matrix a)
    {
      A = a.Copy();
      V = Matrix.Identity(A.Rows);
    }

    /// <summary>Gets or sets the eigenvalues.</summary>
    /// <value>The eigenvalues.</value>
    public Vector Eigenvalues { get; private set; }

    /// <summary>Gets the eigenvectors.</summary>
    /// <value>The eigenvectors.</value>
    public Matrix Eigenvectors { get { return V; } }

    /// <summary>Computes the given tolerance.</summary>
    /// <param name="tol">(Optional) the tolerance.</param>
    public void Compute(double tol = 1.0e-10)
    {
      do
      {
        Factorize();
        // TODO: Fix parallelization
        //if (A.Cols <= 300) // small enough
        //    Factorize();
        //else          // parallelize
        //    Parallel();
      }
      while (Off(A) > tol);

      Sort();
    }


    /// <summary>Factorizes this object.</summary>
    private void Factorize()
    {
      var N = A.Cols;
      for (var p = 0; p < N - 1; p++)
      for (var q = p + 1; q < N; q++)
        Sweep(p, q);
    }

    /// <summary>Offs the given a.</summary>
    /// <param name="a">The int to process.</param>
    /// <returns>A double.</returns>
    private double Off(Matrix a)
    {
      double sum = 0;
      for (var i = 0; i < a.Rows; i++)
      for (var j = 0; j < a.Cols; j++)
        if (i != j)
          sum += sqr(a[i, j]);
      return sqrt(sum);
    }

    /// <summary>Parallels this object.</summary>
    private void Parallel()
    {
      var N = A.Cols;
      // make even pairings
      var n = N % 2 == 0 ? N : N + 1;

      // queue up round-iness of the robin
      var queue = new Queue<int>(n - 1);

      // fill queue
      for (var i = 1; i < N; i++)
        queue.Enqueue(i);
      // add extra for odd pairings
      if (N % 2 == 1)
        queue.Enqueue(-1);

      for (var i = 0; i < n - 1; i++)
      for (var j = 0; j < n / 2; j++)
      {
        int k = n - 1 - j;

        var eK = queue.ElementAt(k - 1);
        var eJ = j == 0 ? 0 : queue.ElementAt(j - 1);

        var p = min(eJ, eK);
        var q = max(eJ, eK);

        // are we in a buy week?
        if (p >= 0)
          Sweep(p, q);
      }

      // move stuff around
      queue.Enqueue(queue.Dequeue());
    }

    /// <summary>Schurs.</summary>
    /// <param name="a">The int to process.</param>
    /// <param name="p">The int to process.</param>
    /// <param name="q">The int to process.</param>
    /// <returns>A Tuple&lt;double,double&gt;</returns>
    private Tuple<double, double> Schur(Matrix a, int p, int q)
    {
      double c, s;
      if (a[q, q] == a[p, p])
      {
        s = c = sqrt(2) / 2;
      }
      else if (a[p, q] != 0)
      {
        var tau = (a[q, q] - a[p, p]) / (2 * a[p, q]);
        double t;
        if (tau >= 0)
          t = 1 / (tau + sqrt(tau + sqr(tau)));
        else
          t = -1 / (-tau + sqrt(1 + sqr(tau)));

        c = 1 / sqrt(1 + sqr(t));
        s = t * c;
      }
      else
      {
        c = 1;
        s = 0;
      }

      return new Tuple<double, double>(c, s);
    }

    /// <summary>Sorts this object.</summary>
    private void Sort()
    {
      //ordering
      var eigs = A.Diag()
                  .Select((d, i) => new Tuple<int, double>(i, d))
                  .OrderByDescending(j => j.Item2)
                  .ToArray();

      // sort eigenvectors
      var copy = V.Copy();
      for (var i = 0; i < eigs.Length; i++)
        copy[i, VectorType.Col] = V[eigs[i].Item1, VectorType.Col];

      // normalize eigenvectors
      copy.Normalize(VectorType.Col);
      V = copy;

      Eigenvalues = eigs.Select(t => t.Item2).ToArray();
    }

    /// <summary>Sweeps.</summary>
    /// <param name="p">The int to process.</param>
    /// <param name="q">The int to process.</param>
    private void Sweep(int p, int q)
    {
      // set jacobi rotation matrix
      var cs = Schur(A, p, q);
      var c = cs.Item1;
      var s = cs.Item2;

      if (c != 1 || s != 0) // if rotation
      {
        /*************************
                         * perform jacobi rotation
                         *************************/
        // calculating intermediate J.T * A
        var pV = Vector.Create(A.Cols, i => A[p, i] * c + A[q, i] * -s);
        var qV = Vector.Create(A.Cols, i => A[q, i] * c + A[p, i] * s);

        // calculating A * J for inner p, q square
        var App = pV[p] * c + pV[q] * -s;
        var Apq = pV[q] * c + pV[p] * s;
        var Aqq = qV[q] * c + qV[p] * s;

        // fill in changes along box
        pV[p] = App;
        pV[q] = qV[p] = Apq;
        qV[q] = Aqq;

        /***************************
         * store accumulated results
         ***************************/
        var pE = Vector.Create(V.Rows, i => V[i, p] * c + V[i, q] * -s);
        var qE = Vector.Create(V.Rows, i => V[i, q] * c + V[i, p] * s);

        /****************
         * matrix updates
         ****************/
        // Update A
        A[p, VectorType.Col] = pV;
        A[p, VectorType.Row] = pV;
        A[q, VectorType.Col] = qV;
        A[q, VectorType.Row] = qV;

        // Update V - not critical 
        V[p, VectorType.Col] = pE;
        V[q, VectorType.Col] = qE;
      }
    }

    #region for brevity...

    /// <summary>Sqrts.</summary>
    /// <param name="x">The x coordinate.</param>
    /// <returns>A double.</returns>
    private double sqrt(double x) { return System.Math.Sqrt(x); }

    /// <summary>Sqrs.</summary>
    /// <param name="x">The x coordinate.</param>
    /// <returns>A double.</returns>
    private double sqr(double x) { return System.Math.Pow(x, 2); }

    /// <summary>Determines the minimum of the given parameters.</summary>
    /// <param name="a">The int to process.</param>
    /// <param name="b">The int to process.</param>
    /// <returns>The minimum value.</returns>
    private int min(int a, int b) { return System.Math.Min(a, b); }

    /// <summary>Determines the maximum of the given parameters.</summary>
    /// <param name="a">The int to process.</param>
    /// <param name="b">The int to process.</param>
    /// <returns>The maximum value.</returns>
    private int max(int a, int b) { return System.Math.Max(a, b); }

    #endregion
  }
}