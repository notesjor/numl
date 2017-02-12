using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using numl.Math.Probability;
using numl.Serialization;

namespace numl.Math.LinearAlgebra
{
  /// <summary>A matrix.</summary>
  public class Matrix
  {
    /// <summary>true to as transpose reference.</summary>
    private bool _asTransposeRef;

    /// <summary>The matrix.</summary>
    private double[][] _matrix;

    //--------------- ctor

    /// <summary>Used only internally.</summary>
    private Matrix() {}

    /// <summary>Create matrix n x n matrix.</summary>
    /// <param name="n">size.</param>
    public Matrix(int n) :
      this(n, n) {}

    /// <summary>Create new n x d matrix.</summary>
    /// <param name="n">rows.</param>
    /// <param name="d">cols.</param>
    public Matrix(int n, int d)
    {
      _asTransposeRef = false;
      Rows = n;
      Cols = d;
      _matrix = new double[n][];
      for (var i = 0; i < n; i++)
        _matrix[i] = new double[d];
    }

    /// <summary>Create new matrix with prepopulated vals.</summary>
    /// <param name="m">initial matrix.</param>
    public Matrix(double[,] m)
    {
      _asTransposeRef = false;
      Rows = m.GetLength(0);
      Cols = m.GetLength(1);
      _matrix = new double[Rows][];
      for (var i = 0; i < Rows; i++)
      {
        _matrix[i] = new double[Cols];
        for (var j = 0; j < Cols; j++)
          _matrix[i][j] = m[i, j];
      }
    }

    /// <summary>Create matrix n x n matrix.</summary>
    /// <exception cref="InvalidOperationException">Thrown when the requested operation is invalid.</exception>
    /// <param name="m">initial matrix.</param>
    public Matrix(double[][] m)
    {
      _asTransposeRef = false;
      Rows = m.GetLength(0);
      if (Rows > 0)
        Cols = m[0].Length;
      else
        throw new InvalidOperationException("Insufficient information to construct Matrix");

      _matrix = m;
    }

    //--------------- access
    /// <summary>Accessor.</summary>
    /// <param name="i">Row.</param>
    /// <param name="j">Column.</param>
    /// <returns>The indexed item.</returns>
    public virtual double this[int i, int j]
    {
      get
      {
        if (!_asTransposeRef)
          return _matrix[i][j];
        return _matrix[j][i];
      }
      set
      {
        if (_asTransposeRef)
          throw new InvalidOperationException("Cannot modify matrix in read-only transpose mode!");

        _matrix[i][j] = value;
      }
    }

    /// <summary>Returns row vector specified at index i.</summary>
    /// <param name="i">row index.</param>
    /// <returns>The indexed item.</returns>
    public virtual Vector this[int i]
    {
      get { return this[i, VectorType.Row]; }
      set { this[i, VectorType.Row] = value; }
    }

    /// <summary>returns col/row vector at index j.</summary>
    /// <param name="i">Col/Row.</param>
    /// <param name="t">Row or Column.</param>
    /// <returns>Vector.</returns>
    public virtual Vector this[int i, VectorType t]
    {
      get
      {
        var dim = t == VectorType.Row ? Rows : Cols;
        if (i >= dim || i < 0)
          throw new IndexOutOfRangeException();

        if (t == VectorType.Row)
          if (_asTransposeRef)
            return new Vector(_matrix, i);
          else
            return new Vector(_matrix[i].ToArray());
        if (_asTransposeRef)
          return new Vector(_matrix, i, true);
        var cols = new double[Rows];
        for (var j = 0; j < Rows; j++)
          cols[j] = _matrix[j][i];

        return new Vector(cols);
      }
      set
      {
        if (_asTransposeRef)
          throw new InvalidOperationException("Cannot modify matrix in read-only transpose mode!");

        var dim1 = t == VectorType.Row ? Rows : Cols;
        var dim2 = t == VectorType.Row ? Cols : Rows;

        if (i >= dim1 || i < 0)
          throw new IndexOutOfRangeException();

        if (value.Length > dim2)
          throw new InvalidOperationException($"Vector has lenght larger then {dim2}");

        if (t == VectorType.Row)
          for (var k = 0; k < Cols; k++)
            _matrix[i][k] = value[k];
        else
          for (var k = 0; k < Rows; k++)
            _matrix[k][i] = value[k];
      }
    }

    /// <summary>Indexer to set items within this collection using array index syntax.</summary>
    /// <param name="f">The Func&lt;double,bool&gt; to process.</param>
    /// <returns>The indexed item.</returns>
    public double this[Func<double, bool> f]
    {
      set
      {
        for (var i = 0; i < Rows; i++)
        for (var j = 0; j < Cols; j++)
          if (f(_matrix[i][j]))
            this[i, j] = value;
      }
    }

    /// <summary>
    ///   Indexer to set items within this collection using an n x 2 array of indices to set.
    /// </summary>
    /// <param name="slice">An n x 2 array of indices to set.</param>
    /// <returns></returns>
    public double this[IEnumerable<int[]> slice]
    {
      set
      {
        foreach (var i in slice)
          this[i[0], i[1]] = value;
      }
    }

    /// <summary>Indexer to get items within this collection using array index syntax.</summary>
    /// <param name="f">The Func&lt;Vector,bool&gt; to process.</param>
    /// <param name="t">The VectorType to process.</param>
    /// <returns>The indexed item.</returns>
    public Matrix this[Func<Vector, bool> f, VectorType t]
    {
      get
      {
        var count = 0;
        if (t == VectorType.Row)
        {
          for (var i = 0; i < Rows; i++)
            if (f(this[i, t]))
              count++;

          var m = new Matrix(count, Cols);
          var j = -1;
          for (var i = 0; i < Rows; i++)
            if (f(this[i, t]))
              m[++j, t] = this[i, t];

          return m;
        }
        else
        {
          for (var i = 0; i < Cols; i++)
            if (f(this[i, t]))
              count++;

          var m = new Matrix(Rows, count);
          var j = -1;
          for (var i = 0; i < Cols; i++)
            if (f(this[i, t]))
              m[++j, t] = this[i, t];

          return m;
        }
      }
    }

    /// <summary>Gets or sets the cols.</summary>
    /// <value>The cols.</value>
    public int Cols { get; private set; }

    /// <summary>Gets or sets the rows.</summary>
    /// <value>The rows.</value>
    public int Rows { get; private set; }

    /// <summary>
    ///   Returns read-only transpose (uses matrix reference to save space)
    ///   It will throw an exception if there is an attempt to write to the matrix.
    /// </summary>
    /// <value>The t.</value>
    public Matrix T
    {
      get
      {
        return new Matrix
        {
          _asTransposeRef = true,
          _matrix = _matrix,
          Cols = Rows,
          Rows = Cols
        };
      }
    }

    /// <summary>Backwards.</summary>
    /// <param name="A">Input Matrix.</param>
    /// <param name="b">The Vector to process.</param>
    /// <returns>A Vector.</returns>
    internal static Vector Backward(Matrix A, Vector b)
    {
      var x = Vector.Zeros(b.Length);
      for (var i = b.Length - 1; i > -1; i--)
      {
        double sum = 0;
        for (var j = i + 1; j < b.Length; j++)
          sum += A[i, j] * x[j];

        x[i] = (b[i] - sum) / A[i, i];
      }

      return x;
    }

    /// <summary>In place centering. WARNING: WILL UPDATE MATRIX!</summary>
    /// <param name="t">.</param>
    /// <returns>A Matrix.</returns>
    public Matrix Center(VectorType t)
    {
      var max = t == VectorType.Row ? Rows : Cols;
      for (var i = 0; i < max; i++)
        this[i, t] -= this[i, t].Mean();
      return this;
    }

    /// <summary>Cholesky Factorization of a Matrix.</summary>
    /// <exception cref="InvalidOperationException">Thrown when the requested operation is invalid.</exception>
    /// <exception cref="SingularMatrixException">Thrown when a Singular Matrix error condition occurs.</exception>
    /// <param name="m">Input Matrix.</param>
    /// <returns>Cholesky Faxtorization (R.T would be other matrix)</returns>
    public static Matrix Cholesky(Matrix m)
    {
      if (m.Rows != m.Cols)
        throw new InvalidOperationException("Factorization requires a symmetric positive semi-definite matrix!");

      var n = m.Rows;
      var A = m.Copy();

      for (var k = 0; k < n; k++)
      {
        if (A[k, k] <= 0)
          throw new SingularMatrixException("Matrix is not symmetric positive semi-definite!");

        A[k, k] = System.Math.Sqrt(A[k, k]);
        for (var j = k + 1; j < n; j++)
          A[j, k] = A[j, k] / A[k, k];

        for (var j = k + 1; j < n; j++)
        for (var i = j; i < n; i++)
          A[i, j] = A[i, j] - A[i, k] * A[j, k];
      }

      // put back zeros...
      for (var i = 0; i < n; i++)
      for (var j = i + 1; j < n; j++)
        A[i, j] = 0;

      return A;
    }

    /// <summary>Cols.</summary>
    /// <param name="i">Zero-based index of the.</param>
    /// <returns>A Vector.</returns>
    public Vector Col(int i)
    {
      return this[i, VectorType.Col];
    }

    /// <summary>create deep copy of matrix.</summary>
    /// <returns>Matrix.</returns>
    public Matrix Copy()
    {
      return ToArray();
    }

    /// <summary>Correlations.</summary>
    /// <param name="source">Source for the.</param>
    /// <param name="t">(Optional) Row or Column sum.</param>
    /// <returns>A Matrix.</returns>
    public static Matrix Correlation(Matrix source, VectorType t = VectorType.Col)
    {
      var length = t == VectorType.Row ? source.Rows : source.Cols;
      var m = new Matrix(length);
      for (var i = 0; i < length; i++)
      for (var j = i; j < length; j++) // symmetric matrix
        m[i, j] = m[j, i] = source[i, t].Correlation(source[j, t]);
      return m;
    }

    /// <summary>Covariances.</summary>
    /// <param name="source">Source for the.</param>
    /// <param name="t">(Optional) Row or Column sum.</param>
    /// <returns>A Matrix.</returns>
    public static Matrix Covariance(Matrix source, VectorType t = VectorType.Col)
    {
      var length = t == VectorType.Row ? source.Rows : source.Cols;
      var m = new Matrix(length);
      //for (int i = 0; i < length; i++)
      for (var i = 0; i < length; i++)
      for (var j = i; j < length; j++)
        m[i, j] = m[j, i] = source[i, t].Covariance(source[j, t]);
      return m;
    }

    /// <summary>Covariance diagram.</summary>
    /// <param name="source">Source for the.</param>
    /// <param name="t">(Optional) Row or Column sum.</param>
    /// <returns>A Vector.</returns>
    public static Vector CovarianceDiag(Matrix source, VectorType t = VectorType.Col)
    {
      var length = t == VectorType.Row ? source.Rows : source.Cols;
      var vector = new Vector(length);
      for (var i = 0; i < length; i++)
        vector[i] = source[i, t].Variance();
      return vector;
    }

    /// <summary>Creates a new Matrix.</summary>
    /// <param name="n">Size.</param>
    /// <param name="f">The Func&lt;int,int,double&gt; to process.</param>
    /// <returns>A Matrix.</returns>
    public static Matrix Create(int n, Func<double> f)
    {
      return Create(n, n, f);
    }

    /// <summary>Creates a new Matrix.</summary>
    /// <param name="n">Size.</param>
    /// <param name="d">cols.</param>
    /// <param name="f">The Func&lt;int,int,double&gt; to process.</param>
    /// <returns>A Matrix.</returns>
    public static Matrix Create(int n, int d, Func<double> f)
    {
      var matrix = new Matrix(n, d);
      for (var i = 0; i < matrix.Rows; i++)
      for (var j = 0; j < matrix.Cols; j++)
        matrix[i, j] = f();
      return matrix;
    }

    /// <summary>Creates a new Matrix.</summary>
    /// <param name="n">Size.</param>
    /// <param name="f">The Func&lt;int,int,double&gt; to process.</param>
    /// <returns>A Matrix.</returns>
    public static Matrix Create(int n, Func<int, int, double> f)
    {
      return Create(n, n, f);
    }

    /// <summary>Creates a new Matrix.</summary>
    /// <param name="n">Size.</param>
    /// <param name="d">cols.</param>
    /// <param name="f">The Func&lt;int,int,double&gt; to process.</param>
    /// <returns>A Matrix.</returns>
    public static Matrix Create(int n, int d, Func<int, int, double> f)
    {
      var matrix = new Matrix(n, d);
      for (var i = 0; i < matrix.Rows; i++)
      for (var j = 0; j < matrix.Cols; j++)
        matrix[i, j] = f(i, j);
      return matrix;
    }

    /// <summary>Dets the given x coordinate.</summary>
    /// <exception cref="InvalidOperationException">Thrown when the requested operation is invalid.</exception>
    /// <param name="x">Matrix x.</param>
    /// <returns>A double.</returns>
    public static double Det(Matrix x)
    {
      //     0, 1, 2
      //    --------
      // 0 | a, b, c
      // 1 | d, e, f
      // 2 | g, h, i

      if (x.Rows != x.Cols)
        throw new InvalidOperationException("Can only compute determinants of square matrices");
      var n = x.Rows;
      if (n == 1)
        return x[0, 0];
      if (n == 2)
        return x[0, 0] * x[1, 1] - x[0, 1] * x[1, 0];
      if (n == 3) // aei + bfg + cdh - ceg - bdi - afh
        return x[0, 0] * x[1, 1] * x[2, 2] +
               x[0, 1] * x[1, 2] * x[2, 0] +
               x[0, 2] * x[1, 0] * x[2, 1] -
               x[0, 2] * x[1, 1] * x[2, 0] -
               x[0, 1] * x[1, 0] * x[2, 2] -
               x[0, 0] * x[1, 2] * x[2, 1];
      // ruh roh, time for generalized determinant
      // to save time we'll do a cholesky factorization
      // (note, this requires a symmetric positive-semidefinite
      // matrix or it might explode....)
      // and square the product of the diagonals
      //return System.Math.Pow(x.Cholesky().Diag().Prod(), 2);

      // switched to QR since it is safer...
      // fyi:  determinant of a triangular matrix is the
      //       product of its diagonals
      // also: det(AB) = det(A) * det(B)
      // that's how we come up with this crazy thing
      var qr = QR(x);
      return qr.Item1.Diag().Prod() * qr.Item2.Diag().Prod();
    }

    /// <summary>Diagrams the given m.</summary>
    /// <param name="m">Input Matrix.</param>
    /// <returns>A Vector.</returns>
    public static Vector Diag(Matrix m)
    {
      var length = m.Cols > m.Rows ? m.Rows : m.Cols;
      var v = Vector.Zeros(length);
      for (var i = 0; i < length; i++)
        v[i] = m[i, i];
      return v;
    }

    /// <summary>Dot product between a matrix and a vector.</summary>
    /// <exception cref="InvalidOperationException">Thrown when the requested operation is invalid.</exception>
    /// <param name="x">Matrix x.</param>
    /// <param name="v">Vector v.</param>
    /// <returns>Vector dot product.</returns>
    public static Vector Dot(Matrix x, Vector v)
    {
      if (v.Length != x.Cols)
        throw new InvalidOperationException("objects are not aligned");

      var toReturn = Vector.Zeros(x.Rows);
      for (var i = 0; i < toReturn.Length; i++)
        toReturn[i] = Vector.Dot(x[i, VectorType.Row], v);
      return toReturn;
    }

    /// <summary>Dot product between a matrix and a vector.</summary>
    /// <exception cref="InvalidOperationException">Thrown when the requested operation is invalid.</exception>
    /// <param name="v">Vector v.</param>
    /// <param name="x">Matrix x.</param>
    /// <returns>Vector dot product.</returns>
    public static Vector Dot(Vector v, Matrix x)
    {
      if (v.Length != x.Rows)
        throw new InvalidOperationException("objects are not aligned");

      var toReturn = Vector.Zeros(x.Cols);
      for (var i = 0; i < toReturn.Length; i++)
        toReturn[i] = Vector.Dot(x[i, VectorType.Col], v);
      return toReturn;
    }

    /// <summary>
    ///   Performs an element wise operation on the input Matrix.
    /// </summary>
    /// <param name="m">Matrix.</param>
    /// <param name="fnElementWiseOp">Function to apply.</param>
    /// <returns>A Matrix.</returns>
    public static Matrix Each(Matrix m, Func<double, double> fnElementWiseOp)
    {
      var copy = m.ToArray();
      for (var i = 0; i < m.Rows; i++)
      for (var j = 0; j < m.Cols; j++)
        copy[i][j] = fnElementWiseOp(copy[i][j]);
      return copy;
    }

    /// <summary>
    ///   Performs an element wise operation on the input Matrix.
    /// </summary>
    /// <param name="m">Matrix.</param>
    /// <param name="fnElementWiseOp">Function to update each cell specified by the value and cell coordinates.</param>
    /// <returns>A Matrix.</returns>
    public static Matrix Each(Matrix m, Func<double, int, int, double> fnElementWiseOp)
    {
      var copy = m.ToArray();
      for (var i = 0; i < m.Rows; i++)
      for (var j = 0; j < m.Cols; j++)
        copy[i][j] = fnElementWiseOp(copy[i][j], i, j);
      return copy;
    }

    /// <summary>
    ///   Performs an element-wise operation on the input Matrices.
    /// </summary>
    /// <param name="m1">First Matrix.</param>
    /// <param name="m2">Second Matrix.</param>
    /// <param name="fnElementWiseOp">Operation to perform on the value from the first and second matrices.</param>
    /// <returns>A Matrix.</returns>
    public static Matrix Each(Matrix m1, Matrix m2, Func<double, double, double> fnElementWiseOp)
    {
      if (m1.Rows != m2.Rows)
        throw new InvalidOperationException("The row dimensions do not match");
      if (m1.Cols != m2.Cols)
        throw new InvalidOperationException("The column dimensions do not match");

      var copy = m1.ToArray();
      for (var i = 0; i < m1.Rows; i++)
      for (var j = 0; j < m1.Cols; j++)
        copy[i][j] = fnElementWiseOp(m1[i, j], m2[i, j]);
      return copy;
    }

    /// <summary>
    ///   Determines whether the specified <see cref="T:System.Object" /> is equal to the current
    ///   <see cref="T:System.Object" />.
    /// </summary>
    /// <param name="m">initial matrix.</param>
    /// <param name="tol">Double to be compared.</param>
    /// <returns>
    ///   true if the specified <see cref="T:System.Object" /> is equal to the current
    ///   <see cref="T:System.Object" />; otherwise, false.
    /// </returns>
    public bool Equals(Matrix m, double tol)
    {
      if (Rows != m.Rows || Cols != m.Cols)
        return false;

      for (var i = 0; i < Rows; i++)
      for (var j = 0; j < Cols; j++)
        if (System.Math.Abs(this[i, j] - m[i, j]) > tol)
          return false;
      return true;
    }

    /// <summary>
    ///   Determines whether the specified <see cref="T:System.Object" /> is equal to the current
    ///   <see cref="T:System.Object" />.
    /// </summary>
    /// <param name="obj">The object to compare with the current object.</param>
    /// <returns>
    ///   true if the specified <see cref="T:System.Object" /> is equal to the current
    ///   <see cref="T:System.Object" />; otherwise, false.
    /// </returns>
    public override bool Equals(object obj)
    {
      if (obj is Matrix)
      {
        var m = obj as Matrix;
        if (Rows != m.Rows || Cols != m.Cols)
          return false;

        for (var i = 0; i < Rows; i++)
        for (var j = 0; j < Cols; j++)
          if (this[i, j] != m[i, j])
            return false;

        return true;
      }
      return false;
    }

    /// <summary>Eigen Decomposition.</summary>
    /// <param name="A">Input Matrix.</param>
    /// <returns>Tuple(Eigen Values, Eigen Vectors)</returns>
    public static Tuple<Vector, Matrix> Evd(Matrix A)
    {
      var eigs = new Evd(A);
      eigs.Compute();
      return new Tuple<Vector, Matrix>(eigs.Eigenvalues, eigs.Eigenvectors);
    }

    /// <summary>Extracts this object.</summary>
    /// <param name="m">Input Matrix.</param>
    /// <param name="x">Matrix x.</param>
    /// <param name="y">The y coordinate.</param>
    /// <param name="width">The width.</param>
    /// <param name="height">The height.</param>
    /// <param name="safe">(Optional) true to safe.</param>
    /// <returns>A Matrix.</returns>
    public static Matrix Extract(Matrix m, int x, int y, int width, int height, bool safe = true)
    {
      var m2 = Zeros(height, width);
      for (var i = y; i < y + height; i++)
      for (var j = x; j < x + width; j++)
        if (safe && i < m.Rows && j < m.Cols)
          m2[i - y, j - x] = m[i, j];

      return m2;
    }

    /// <summary>Forwards.</summary>
    /// <param name="A">Input Matrix.</param>
    /// <param name="b">The Vector to process.</param>
    /// <returns>A Vector.</returns>
    internal static Vector Forward(Matrix A, Vector b)
    {
      var x = Vector.Zeros(b.Length);
      for (var i = 0; i < b.Length; i++)
      {
        double sum = 0;
        for (var j = 0; j < i; j++)
          sum += A[i, j] * x[j];

        x[i] = (b[i] - sum) / A[i, i];
      }

      return x;
    }

    /// <summary>Matrix Frobenius Norm.</summary>
    /// <param name="A">Input Matrix.</param>
    /// <returns>Frobenius Norm (double)</returns>
    public static double FrobeniusNorm(Matrix A)
    {
      return System.Math.Sqrt((A.T * A).Trace());
    }

    /// <summary>Gets the cols in this collection.</summary>
    /// <returns>
    ///   An enumerator that allows foreach to be used to process the cols in this collection.
    /// </returns>
    public IEnumerable<Vector> GetCols()
    {
      for (var i = 0; i < Cols; i++)
        yield return this[i, VectorType.Col];
    }

    /// <summary>Serves as a hash function for a particular type.</summary>
    /// <returns>A hash code for the current <see cref="T:System.Object" />.</returns>
    public override int GetHashCode()
    {
      return _matrix.GetHashCode();
    }

    /// <summary>Gets a matrix.</summary>
    /// <param name="d1">The first int.</param>
    /// <param name="d2">The second int.</param>
    /// <param name="n1">The first int.</param>
    /// <param name="n2">The second int.</param>
    /// <returns>The matrix.</returns>
    public Matrix GetMatrix(int d1, int d2, int n1, int n2)
    {
      var rows = n2 - n1 + 1;
      var cols = d2 - d1 + 1;
      var m = new double[rows][];
      for (var i = 0; i < rows; i++)
      {
        m[i] = new double[cols];
        for (var j = 0; j < cols; j++)
          m[i][j] = this[i + n1, j + d1];
      }
      return m;
    }

    /// <summary>Gets the rows in this collection.</summary>
    /// <returns>
    ///   An enumerator that allows foreach to be used to process the rows in this collection.
    /// </returns>
    public IEnumerable<Vector> GetRows()
    {
      for (var i = 0; i < Rows; i++)
        yield return this[i, VectorType.Row];
    }

    /// <summary>Gets a vector.</summary>
    /// <param name="index">Zero-based index of the.</param>
    /// <param name="from">Source for the.</param>
    /// <param name="to">to.</param>
    /// <param name="type">The type.</param>
    /// <returns>The vector.</returns>
    public Vector GetVector(int index, int from, int to, VectorType type)
    {
      var v = new double[to - from + 1];
      for (int i = from, j = 0; i < to + 1; i++, j++)
        v[j] = this[index, type][i];
      return new Vector(v);
    }

    /// <summary>n x d identity matrix.</summary>
    /// <param name="n">rows.</param>
    /// <param name="d">cols.</param>
    /// <returns>Matrix.</returns>
    public static Matrix Identity(int n, int d)
    {
      var m = new double[n][];
      for (var i = 0; i < n; i++)
      {
        m[i] = new double[d];
        for (var j = 0; j < d; j++)
          if (i == j)
            m[i][j] = 1;
          else
            m[i][j] = 0;
      }

      return new Matrix
      {
        _matrix = m,
        Rows = n,
        Cols = d,
        _asTransposeRef = false
      };
    }

    /// <summary>n x n identity matrix.</summary>
    /// <param name="n">Size.</param>
    /// <returns>Matrix.</returns>
    public static Matrix Identity(int n)
    {
      return Identity(n, n);
    }

    /// <summary>Enumerates indices in this collection.</summary>
    /// <param name="source">Source for the.</param>
    /// <param name="f">The Func&lt;Vector,bool&gt; to process.</param>
    /// <param name="t">Row or Column sum.</param>
    /// <returns>
    ///   An enumerator that allows foreach to be used to process indices in this collection.
    /// </returns>
    public static IEnumerable<int> Indices(Matrix source, Func<Vector, bool> f, VectorType t = VectorType.Row)
    {
      var max = t == VectorType.Row ? source.Rows : source.Cols;
      for (var i = 0; i < max; i++)
        if (f(source[i, t]))
          yield return i;
    }

    /// <summary>
    ///   Returns a new Matrix with the Vector inserted at the specified position
    /// </summary>
    /// <param name="v">Vector to insert</param>
    /// <param name="index">The zero based row / column.</param>
    /// <param name="t">Vector orientation</param>
    /// <param name="insertAfter">Insert after or before the last row / column</param>
    /// <returns></returns>
    public Matrix Insert(Vector v, int index, VectorType t, bool insertAfter = true)
    {
      if (t == VectorType.Col && v.Length != Rows)
        throw new ArgumentException("Column vector does not match matrix height");
      if (t == VectorType.Row && v.Length != Cols)
        throw new ArgumentException("Row vector does not match matrix width");

      if (t == VectorType.Col && (index >= Cols || index < 0) && (index != -1 || !insertAfter))
        throw new ArgumentException("Column index does not match matrix width");
      if (t == VectorType.Row && (index >= Rows || index < 0) && (index != -1 || !insertAfter))
        throw new ArgumentException("Row index does not match matrix height");

      var temp = ToArray().ToList();
      if (t == VectorType.Row)
      {
        if (index == temp.Count - 1 && insertAfter)
          temp.Add(v);
        else
          temp.Insert(index, v);
      }
      else
      {
        if (index == temp[0].Length - 1 && insertAfter)
          for (var i = 0; i < temp.Count; i++)
          {
            var copy = temp[i].ToList();
            copy.Add(v[i]);
            temp[i] = copy.ToArray();
          }
        else
          for (var i = 0; i < temp.Count; i++)
          {
            var copy = temp[i].ToList();
            copy.Insert(index, v[i]);
            temp[i] = copy.ToArray();
          }
      }

      return new Matrix(temp.ToArray());
    }

    /// <summary>
    ///   Creates an inverse of the current matrix
    /// </summary>
    /// <returns></returns>
    public Matrix Inverse()
    {
      return Inverse(this);
    }

    /// <summary>Inverses the given matrix.</summary>
    /// <exception cref="SingularMatrixException">Thrown when a Singular Matrix error condition occurs.</exception>
    /// <param name="mat">Matrix.</param>
    /// <returns>A Matrix.</returns>
    private static Matrix Inverse(Matrix mat)
    {
      // working space
      var matrix = new Matrix(mat.Rows, 2 * mat.Cols);
      // copy over colums
      for (var i = 0; i < mat.Cols; i++)
        matrix[i, VectorType.Col] = mat[i, VectorType.Col];

      // fill in identity
      for (var i = mat.Cols; i < 2 * mat.Cols; i++)
        matrix[i - mat.Cols, i] = 1;

      double c;
      for (var y = 0; y < matrix.Rows; y++)
      {
        var maxrow = y;
        for (var y2 = y + 1; y2 < matrix.Rows; y2++)
          if (System.Math.Abs(matrix[y2, y]) > System.Math.Abs(matrix[maxrow, y]))
            maxrow = y2;

        // swap rows
        matrix.SwapRow(maxrow, y);

        // uh oh
        if (System.Math.Abs(matrix[y][y]) <= 0.00000000001)
          throw new SingularMatrixException("Matrix is becoming unstable!");

        for (var y2 = y + 1; y2 < matrix.Rows; y2++)
        {
          c = matrix[y2, y] / matrix[y, y];
          for (var x = y; x < matrix.Cols; x++)
            matrix[y2, x] -= matrix[y, x] * c;
        }
      }

      // back substitute
      for (var y = matrix.Rows - 1; y >= 0; y--)
      {
        c = matrix[y][y];
        for (var y2 = 0; y2 < y; y2++)
        for (var x = matrix.Cols - 1; x > y - 1; x--)
          matrix[y2, x] -= matrix[y, x] * matrix[y2, y] / c;

        matrix[y, y] /= c;
        for (var x = matrix.Rows; x < matrix.Cols; x++)
          matrix[y, x] /= c;
      }

      // generate result
      var result = new Matrix(mat.Rows);
      for (var i = mat.Cols; i < 2 * mat.Cols; i++)
        result[i - mat.Cols, VectorType.Col] = matrix[i, VectorType.Col];

      return result;
    }

    /// <summary>Loads the given stream.</summary>
    /// <exception cref="InvalidOperationException">Thrown when the requested file is not present.</exception>
    /// <param name="file">The file to load.</param>
    /// <returns>A Matrix.</returns>
    public static Matrix Load(string file)
    {
      if (File.Exists(file))
        using (var fs = new FileStream(file, FileMode.Open))
        {
          using (var f = new StreamReader(fs))
          {
            using (var r = new JsonReader(f))
            {
              return r.ReadMatrix();
            }
          }
        }
      throw new InvalidOperationException("File not found");
    }

    /// <summary>NOT IMPLEMENTED!</summary>
    /// <exception cref="InvalidOperationException">Thrown when the requested operation is invalid.</exception>
    /// <param name="A">.</param>
    /// <returns>A Tuple&lt;Matrix,Matrix,Matrix&gt;</returns>
    public static Tuple<Matrix, Matrix, Matrix> LU(Matrix A)
    {
      // TODO: FINISH ALGORITHM
      if (A.Rows != A.Cols)
        throw new InvalidOperationException("Factorization requires a symmetric positive semidefinite matrix!");

      var n = A.Rows;

      var P = Pivot(A);
      var M = P * A;

      var L = Identity(n);
      var U = Zeros(n);

      for (var j = 0; j < n; j++)
      {
        L[j, j] = 1;

        for (var i = 0; i < j + 1; i++)
        {
          U[i, j] = M[i, j];
          for (var k = 0; k < i; k++)
            U[i, j] -= U[k, j] * L[i, k];
        }


        for (var i = j; i < n; i++)
        {
          L[i, j] = M[i, j];
          for (var k = 0; k < j; k++)
            L[i, j] -= U[k, j] * L[i, k];

          if (U[j, j] == 0)
            Debug.WriteLine("Unstable divisor...");

          L[i, j] /= U[j, j];
        }
      }

      return new Tuple<Matrix, Matrix, Matrix>(P, L, U);
    }

    /// <summary>
    ///   Returns a vector of the maximum values for each row/column.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="t">Use column or row (default: Col)</param>
    /// <returns></returns>
    public static Vector Max(Matrix source, VectorType t = VectorType.Col)
    {
      var num = t == VectorType.Row ? source.Cols : source.Rows;
      var vectorType = t == VectorType.Row ? VectorType.Col : VectorType.Row;
      var vectors = new Vector(num);
      for (var i = 0; i < num; i++)
        vectors[i] = source[i, vectorType].Max();
      return vectors;
    }

    /// <summary>Determines the maximum of the given parameters.</summary>
    /// <param name="source">Source for the.</param>
    /// <returns>The maximum value.</returns>
    public static double Max(Matrix source)
    {
      var max = double.MinValue;
      for (var i = 0; i < source.Rows; i++)
      for (var j = 0; j < source.Cols; j++)
        if (source[i, j] > max)
          max = source[i, j];

      return max;
    }

    /// <summary>Determines the mean of the given parameters.</summary>
    /// <param name="source">Source for the.</param>
    /// <param name="t">Row or Column sum.</param>
    /// <returns>The mean value.</returns>
    public static Vector Mean(Matrix source, VectorType t)
    {
      var count = t == VectorType.Row ? source.Cols : source.Rows;
      var type = t == VectorType.Row ? VectorType.Col : VectorType.Row;
      var v = new Vector(count);
      for (var i = 0; i < count; i++)
        v[i] = source[i, type].Mean();
      return v;
    }

    /// <summary>
    ///   Returns a vector of the median values for each row or column.
    /// </summary>
    /// <param name="source">Matrix.</param>
    /// <param name="t">VectorType.</param>
    /// <returns></returns>
    public static Vector Median(Matrix source, VectorType t = VectorType.Col)
    {
      var vectors = t == VectorType.Row ? source.GetCols() : source.GetRows();
      return vectors.Select(s => s.Median()).ToVector();
    }

    /// <summary>
    ///   Returns a vector of the minimum values for each row/column.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="t">Use column or row (default: Col)</param>
    /// <returns></returns>
    public static Vector Min(Matrix source, VectorType t = VectorType.Col)
    {
      var num = t == VectorType.Row ? source.Cols : source.Rows;
      var vectorType = t == VectorType.Row ? VectorType.Col : VectorType.Row;
      var vectors = new Vector(num);
      for (var i = 0; i < num; i++)
        vectors[i] = source[i, vectorType].Min();
      return vectors;
    }

    /// <summary>Determines the minimum of the given parameters.</summary>
    /// <param name="source">Source for the.</param>
    /// <returns>The minimum value.</returns>
    public static double Min(Matrix source)
    {
      var min = double.MaxValue;
      for (var i = 0; i < source.Rows; i++)
      for (var j = 0; j < source.Cols; j++)
        if (source[i, j] < min)
          min = source[i, j];

      return min;
    }

    /// <summary>Standard Matrix Norm.</summary>
    /// <param name="A">Input Matrix.</param>
    /// <param name="p">The double to process.</param>
    /// <returns>Standard Norm (double)</returns>
    public static double Norm(Matrix A, double p)
    {
      double norm = 0;
      for (var i = 0; i < A.Rows; i++)
      for (var j = 0; j < A.Cols; j++)
        norm += System.Math.Pow(System.Math.Abs(A[i, j]), p);
      return System.Math.Pow(norm, 1d / p);
    }

    //-------------- destructive ops
    /// <summary>In place normalization. WARNING: WILL UPDATE MATRIX!</summary>
    /// <param name="t">.</param>
    public void Normalize(VectorType t)
    {
      var max = t == VectorType.Row ? Rows : Cols;
      for (var i = 0; i < max; i++)
        this[i, t] /= this[i, t].Norm();
    }

    /// <summary>Normalise random.</summary>
    /// <param name="n">Size.</param>
    /// <param name="d">cols.</param>
    /// <param name="min">(Optional) the minimum.</param>
    /// <returns>A Matrix.</returns>
    public static Matrix NormRand(int n, int d, double min = 0)
    {
      var m = new double[n][];
      for (var i = 0; i < n; i++)
      {
        m[i] = new double[d];
        for (var j = 0; j < d; j++)
          m[i][j] = Sampling.GetNormal() + min;
      }

      return new Matrix {_matrix = m, _asTransposeRef = false, Cols = d, Rows = n};
    }

    /// <summary>Normalise random.</summary>
    /// <param name="n">Size.</param>
    /// <param name="min">(Optional) the minimum.</param>
    /// <returns>A Matrix.</returns>
    public static Matrix NormRand(int n, double min = 0)
    {
      return NormRand(n, n, min);
    }

    /// <summary>Normalise random.</summary>
    /// <exception cref="InvalidOperationException">Thrown when the requested operation is invalid.</exception>
    /// <param name="means">The means.</param>
    /// <param name="stdDev">The standard development.</param>
    /// <param name="n">Size.</param>
    /// <returns>A Matrix.</returns>
    public static Matrix NormRand(Vector means, Vector stdDev, int n)
    {
      if (means.Length != stdDev.Length)
        throw new InvalidOperationException("Invalid Dimensionality");

      var d = means.Length;
      var m = new double[n][];

      for (var i = 0; i < n; i++)
      {
        m[i] = new double[d];
        for (var j = 0; j < d; j++)
          m[i][j] = Sampling.GetNormal(means[j], stdDev[j]);
      }

      return new Matrix {_matrix = m, _asTransposeRef = false, Cols = d, Rows = n};
    }

    /// <summary>Addition operator.</summary>
    /// <exception cref="InvalidOperationException">Thrown when the requested operation is invalid.</exception>
    /// <param name="m1">The first Matrix.</param>
    /// <param name="m2">The second Matrix.</param>
    /// <returns>The result of the operation.</returns>
    public static Matrix operator +(Matrix m1, Matrix m2)
    {
      if (m1.Rows != m2.Rows || m1.Cols != m2.Cols)
        throw new InvalidOperationException("Dimensions do not match");

      var result = new double[m1.Rows][];
      for (var i = 0; i < m1.Rows; i++)
      {
        result[i] = new double[m1.Cols];
        for (var j = 0; j < m1.Cols; j++)
          result[i][j] = m1[i, j] + m2[i, j];
      }

      return result;
    }

    /// <summary>In memory addition of double to matrix.</summary>
    /// <param name="m">Matrix.</param>
    /// <param name="s">double.</param>
    /// <returns>The result of the operation.</returns>
    public static Matrix operator +(Matrix m, double s)
    {
      var result = new double[m.Rows][];
      for (var i = 0; i < m.Rows; i++)
      {
        result[i] = new double[m.Cols];
        for (var j = 0; j < m.Cols; j++)
          result[i][j] = m[i][j] + s;
      }
      return result;
    }

    /// <summary>Addition operator.</summary>
    /// <param name="s">The double to process.</param>
    /// <param name="m">The Matrix to process.</param>
    /// <returns>The result of the operation.</returns>
    public static Matrix operator +(double s, Matrix m)
    {
      return m + s;
    }

    /// <summary>
    ///   Solves Ax = b for x If A is not square or the system is overdetermined, this operation solves
    ///   the linear least squares A.T * A x = A.T * b.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when the requested operation is invalid.</exception>
    /// <param name="A">Matrix A.</param>
    /// <param name="b">Vector b.</param>
    /// <returns>x.</returns>
    public static Vector operator /(Matrix A, Vector b)
    {
      if (A.Rows != b.Length)
        throw new InvalidOperationException("Matrix row count does not match vector length!");

      // LLS
      if (A.Rows != A.Cols)
      {
        var C = A.T * A;
        var L = C.Cholesky();
        var d = (A.T * b).ToVector();
        var z = Forward(L, d);
        var x = Backward(L.T, z);
        return x;
      }
      // regular solve
      // need to be smarter here....
      return ((A ^ -1) * b).ToVector();
    }

    /// <summary>Division operator.</summary>
    /// <param name="A">The Matrix to process.</param>
    /// <param name="b">The double to process.</param>
    /// <returns>The result of the operation.</returns>
    public static Matrix operator /(Matrix A, double b)
    {
      var result = new double[A.Rows][];
      for (var i = 0; i < A.Rows; i++)
      {
        result[i] = new double[A.Cols];
        for (var j = 0; j < A.Cols; j++)
          result[i][j] = A[i, j] / b;
      }
      return result;
    }

    // --------------------- mathematical ops
    /// <summary>Equality operator.</summary>
    /// <param name="m1">The first Matrix.</param>
    /// <param name="m2">The second Matrix.</param>
    /// <returns>The result of the operation.</returns>
    public static bool operator ==(Matrix m1, Matrix m2)
    {
      return ReferenceEquals(m1, null) && ReferenceEquals(m2, null) || m1.Equals(m2);
    }

    /// <summary>
    ///   Matrix inverse using pivoted Gauss-Jordan elimination with partial pivoting
    ///   See:http://www.cse.illinois.edu/iem/linear_equations/gauss_jordan/for python implementaion.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when the requested operation is invalid.</exception>
    /// <param name="mat">Matrix.</param>
    /// <param name="n">-1.</param>
    /// <returns>Inverse (or exception if matrix is singular)</returns>
    public static Matrix operator ^(Matrix mat, int n)
    {
      if (mat.Rows != mat.Cols)
        throw new InvalidOperationException("Can only find powers of square matrices!");

      if (n == 0)
        return Identity(mat.Rows, mat.Cols);
      if (n == 1)
        return mat.Copy();
      if (n == -1)
        return Inverse(mat);
      var negative = n < 0;
      var pow = System.Math.Abs(n);
      var scratch = mat.Copy();
      for (var i = 0; i < pow; i++)
        scratch = scratch * mat;

      if (!negative)
        return scratch;
      return Inverse(scratch);
    }

    // --------------------- implicity operators
    /// <summary>Matrix casting operator.</summary>
    /// <param name="m">Matrix.</param>
    public static implicit operator Matrix(double[,] m)
    {
      return new Matrix(m);
    }

    /// <summary>Matrix casting operator.</summary>
    /// <param name="m">Matrix.</param>
    public static implicit operator Matrix(int[,] m)
    {
      var rows = m.GetLength(0);
      var cols = m.GetLength(1);
      var matrix = new double[rows][];
      for (var i = 0; i < rows; i++)
      {
        matrix[i] = new double[cols];
        for (var j = 0; j < cols; j++)
          matrix[i][j] = m[i, j];
      }
      return matrix;
    }

    /// <summary>
    ///   Performs an implicit conversion from double[][] to <see cref="Matrix" />.
    /// </summary>
    /// <param name="m">The m.</param>
    /// <returns>The result of the conversion.</returns>
    public static implicit operator Matrix(double[][] m)
    {
      return new Matrix(m);
    }

    /// <summary>Inequality operator.</summary>
    /// <param name="m1">The first Matrix.</param>
    /// <param name="m2">The second Matrix.</param>
    /// <returns>The result of the operation.</returns>
    public static bool operator !=(Matrix m1, Matrix m2)
    {
      return !m1.Equals(m2);
    }

    /// <summary>matrix multiplication.</summary>
    /// <exception cref="InvalidOperationException">Thrown when the requested operation is invalid.</exception>
    /// <param name="m1">left hand side.</param>
    /// <param name="m2">right hand side.</param>
    /// <returns>matrix.</returns>
    public static Matrix operator *(Matrix m1, Matrix m2)
    {
      if (m1.Cols != m2.Rows)
        throw new InvalidOperationException("Invalid multiplication dimenstion");

      var result = new double[m1.Rows][];
      for (var i = 0; i < m1.Rows; i++)
      {
        result[i] = new double[m2.Cols];
        for (var j = 0; j < m2.Cols; j++)
        for (var k = 0; k < m1.Cols; k++)
          result[i][j] += m1[i, k] * m2[k, j];
      }

      return result;
    }

    /// <summary>Scalar matrix multiplication.</summary>
    /// <param name="s">scalar.</param>
    /// <param name="m">matrix.</param>
    /// <returns>matrix.</returns>
    public static Matrix operator *(double s, Matrix m)
    {
      var result = new double[m.Rows][];
      for (var i = 0; i < m.Rows; i++)
      {
        result[i] = new double[m.Cols];
        for (var j = 0; j < m.Cols; j++)
          result[i][j] = s * m[i, j];
      }

      return result;
    }

    /// <summary>reverse.</summary>
    /// <param name="m">.</param>
    /// <param name="s">.</param>
    /// <returns>The result of the operation.</returns>
    public static Matrix operator *(Matrix m, double s)
    {
      return s * m;
    }

    /// <summary>Multiplication operator.</summary>
    /// <param name="m">The Matrix to process.</param>
    /// <param name="v">The Vector to process.</param>
    /// <returns>The result of the operation.</returns>
    public static Matrix operator *(Matrix m, Vector v)
    {
      var ans = Dot(m, v);
      return ans.ToMatrix(VectorType.Col);
    }

    /// <summary>Multiplication operator.</summary>
    /// <param name="v">The Vector to process.</param>
    /// <param name="m">The Matrix to process.</param>
    /// <returns>The result of the operation.</returns>
    public static Matrix operator *(Vector v, Matrix m)
    {
      var ans = Dot(v, m);
      return ans.ToMatrix(VectorType.Row);
    }

    /// <summary>Subtraction operator.</summary>
    /// <exception cref="InvalidOperationException">Thrown when the requested operation is invalid.</exception>
    /// <param name="m1">The first Matrix.</param>
    /// <param name="m2">The second Matrix.</param>
    /// <returns>The result of the operation.</returns>
    public static Matrix operator -(Matrix m1, Matrix m2)
    {
      if (m1.Rows != m2.Rows || m1.Cols != m2.Cols)
        throw new InvalidOperationException("Dimensions do not match");

      var result = new double[m1.Rows][];
      for (var i = 0; i < m1.Rows; i++)
      {
        result[i] = new double[m1.Cols];
        for (var j = 0; j < m1.Cols; j++)
          result[i][j] = m1[i, j] - m2[i, j];
      }

      return result;
    }

    /// <summary>Subtract double from every element in the Matrix.</summary>
    /// <param name="m">Matrix.</param>
    /// <param name="s">Double.</param>
    /// <returns>The result of the operation.</returns>
    public static Matrix operator -(Matrix m, double s)
    {
      var result = new double[m.Rows][];
      for (var i = 0; i < m.Rows; i++)
      {
        result[i] = new double[m.Cols];
        for (var j = 0; j < m.Cols; j++)
          result[i][j] = m[i, j] - s;
      }
      return result;
    }

    /// <summary>Subtraction operator.</summary>
    /// <param name="s">The double to process.</param>
    /// <param name="m">The Matrix to process.</param>
    /// <returns>The result of the operation.</returns>
    public static Matrix operator -(double s, Matrix m)
    {
      // subtracting matrix so every item
      // in matrix is negated and s is added
      return -1 * m + s;
    }

    /// <summary>
    ///   Parses a string containing MATLAB style Matrix syntax, i.e. "[[1, 2, 3]; [3, 4, 5]]"
    /// </summary>
    /// <param name="text">Input string to parse.</param>
    /// <returns>Matrix.</returns>
    public static Matrix Parse(string text)
    {
      var arrs = text.Split(new[] {'[', ';', ']', '\r', '\n', '\t'}, StringSplitOptions.RemoveEmptyEntries)
                     .Where(w => !string.IsNullOrWhiteSpace(w)).ToArray();

      var rows = arrs.Length;

      var result = new double[rows][];

      for (var i = 0; i < rows; i++)
        result[i] = arrs[i].Trim().Split(new[] {',', ' '}, StringSplitOptions.RemoveEmptyEntries)
                           .Select(s => double.Parse(s.Trim())).ToArray();

      return new Matrix(result);
    }

    /// <summary>Pivots the given m.</summary>
    /// <exception cref="InvalidOperationException">Thrown when the requested operation is invalid.</exception>
    /// <param name="M">The Matrix to process.</param>
    /// <returns>A Matrix.</returns>
    public static Matrix Pivot(Matrix M)
    {
      if (M.Rows != M.Cols)
        throw new InvalidOperationException("Factorization requires a symmetric positive semidefinite matrix!");

      var m = M.Rows;
      var P = Identity(m);
      for (var j = 0; j < m; j++)
      {
        var row = new Tuple<int, double>(j, 0);
        for (var i = j; i < m; i++)
          if (row.Item2 < System.Math.Abs(M[i, j]))
            row = new Tuple<int, double>(i, System.Math.Abs(M[i, j]));

        if (row.Item1 != j)
          P.SwapRow(j, row.Item1);
      }

      return P;
    }

    /// <summary>Modified Gram-Schmidt QR Factorization.</summary>
    /// <param name="A">Matrix A.</param>
    /// <returns>Tuple(Q, R)</returns>
    public static Tuple<Matrix, Matrix> QR(Matrix A)
    {
      var n = A.Rows;
      var R = Zeros(n);
      var Q = A.Copy();
      for (var k = 0; k < n; k++)
      {
        R[k, k] = Q[k, VectorType.Col].Norm();
        Q[k, VectorType.Col] = Q[k, VectorType.Col] / R[k, k];

        for (var j = k + 1; j < n; j++)
        {
          R[k, j] = Vector.Dot(Q[k, VectorType.Col], A[j, VectorType.Col]);
          Q[j, VectorType.Col] = Q[j, VectorType.Col] - R[k, j] * Q[k, VectorType.Col];
        }
      }

      return new Tuple<Matrix, Matrix>(Q, R);
    }

    /// <summary>
    ///   Generate a matrix n x d with numbers 0 less than x less than 1 drawn uniformly at random.
    /// </summary>
    /// <param name="n">rows.</param>
    /// <param name="d">cols.</param>
    /// <param name="min">(Optional) the minimum.</param>
    /// <returns>n x d Matrix.</returns>
    public static Matrix Rand(int n, int d, double min = 0)
    {
      var m = new double[n][];
      for (var i = 0; i < n; i++)
      {
        m[i] = new double[d];
        for (var j = 0; j < d; j++)
          m[i][j] = Sampling.GetUniform() + min;
      }

      return new Matrix {_matrix = m, _asTransposeRef = false, Cols = d, Rows = n};
    }

    /// <summary>
    ///   Generate a matrix n x d with numbers 0 less than x less than 1 drawn uniformly at random.
    /// </summary>
    /// <param name="n">rows.</param>
    /// <param name="min">(Optional) the minimum.</param>
    /// <returns>n x d Matrix.</returns>
    public static Matrix Rand(int n, double min = 0)
    {
      return Rand(n, n, min);
    }

    /// <summary>Removes this object.</summary>
    /// <param name="index">Zero-based index of the.</param>
    /// <param name="t">.</param>
    /// <returns>A Matrix.</returns>
    public Matrix Remove(int index, VectorType t)
    {
      var max = t == VectorType.Row ? Rows : Cols;
      var row = t == VectorType.Row ? Rows - 1 : Rows;
      var col = t == VectorType.Col ? Cols - 1 : Cols;

      var m = new Matrix(row, col);
      var j = -1;
      for (var i = 0; i < max; i++)
      {
        if (i == index)
          continue;
        m[++j, t] = this[i, t];
      }

      return m;
    }

    /// <summary>
    ///   Reshapes the supplied Vector into a Matrix form.
    /// </summary>
    /// <param name="v">Source vector to act on.</param>
    /// <param name="dimension">Length of the specified dimension.</param>
    /// <param name="dimensionType">Dimension type to use for creating a <paramref name="dimension" /> by n matrix.</param>
    /// <param name="byVector">Direction to process, i.e. Row = Fill Down then Right, or Col = Fill Right then Down</param>
    /// <returns></returns>
    public static Matrix Reshape(
      Vector v,
      int dimension,
      VectorType dimensionType = VectorType.Col,
      VectorType byVector = VectorType.Row)
    {
      var x = dimensionType == VectorType.Row ? dimension : v.Length / dimension;
      var y = dimensionType == VectorType.Col ? dimension : v.Length / dimension;
      return Reshape(v, x, y, byVector);
    }

    /// <summary>
    ///   Reshapes the supplied Vector into a Matrix form.
    /// </summary>
    /// <param name="v">Vector to reshape.</param>
    /// <param name="rows">Height of the matrix to return</param>
    /// <param name="columns">Width of the matrix to return</param>
    /// <param name="byVector">Direction to process, i.e. Row = Fill Down then Right, or Col = Fill Right then Down</param>
    /// <returns></returns>
    public static Matrix Reshape(Vector v, int rows, int columns, VectorType byVector = VectorType.Row)
    {
      if (rows * columns != v.Length)
        throw new InvalidOperationException(
          $"Cannot reshape Vector of length {v.Length} into a {rows} x {columns} Matrix.");

      var m = new Matrix(rows, columns);

      var counter = 0;

      switch (byVector)
      {
        case VectorType.Row:
        {
          for (var i = 0; i < columns; i++)
          for (var k = 0; k < rows; k++)
            m[k, i] = v[counter++];
        }
          break;
        case VectorType.Col:
        {
          for (var i = 0; i < rows; i++)
          for (var k = 0; k < columns; k++)
            m[i, k] = v[counter++];
        }
          break;
      }

      return m;
    }

    /// <summary>
    ///   Reshapes the supplied matrix into a new matrix shape.
    /// </summary>
    /// <param name="m">Matrix to reshape.</param>
    /// <param name="rows">Number of rows of the new matrix.</param>
    /// <param name="cols">Number of columns of the new matrix.</param>
    /// <returns>Matrix.</returns>
    public static Matrix Reshape(Matrix m, int rows, int cols)
    {
      var result = new Matrix(rows, cols);

      var width = rows > m.Rows ? m.Rows : rows;
      var height = cols > m.Cols ? m.Cols : cols;

      for (var y = 0; y < height; y++)
      for (var x = 0; x < width; x++)
        result[y, x] = m[y, x];

      return result;
    }

    /// <summary>Enumerates reverse in this collection.</summary>
    /// <param name="source">Source for the.</param>
    /// <param name="t">(Optional) Row or Column sum.</param>
    /// <returns>
    ///   An enumerator that allows foreach to be used to process reverse in this collection.
    /// </returns>
    public static IEnumerable<Vector> Reverse(Matrix source, VectorType t = VectorType.Row)
    {
      var length = t == VectorType.Row ? source.Rows : source.Cols;
      for (var i = length - 1; i > -1; i--)
        yield return source[i, t];
    }

    /// <summary>Matrix Roundoff.</summary>
    /// <param name="m">Input Matrix.</param>
    /// <param name="decimals">(Optional) Max number of decimals (default 0 - integral members)</param>
    /// <returns>Rounded Matrix.</returns>
    public static Matrix Round(Matrix m, int decimals = 0)
    {
      for (var i = 0; i < m.Rows; i++)
      for (var j = 0; j < m.Cols; j++)
        m[i, j] = System.Math.Round(m[i, j], decimals);
      return m;
    }

    /// <summary>Rows.</summary>
    /// <param name="i">Zero-based index of the.</param>
    /// <returns>A Vector.</returns>
    public Vector Row(int i)
    {
      return this[i, VectorType.Row];
    }

    /// <summary>
    ///   Save matrix to file
    /// </summary>
    /// <param name="file">file to save</param>
    public void Save(string file)
    {
      using (var fs = new FileStream(file, FileMode.CreateNew))
      {
        using (var f = new StreamWriter(fs))
        {
          using (var w = new JsonWriter(f))
          {
            w.WriteMatrix(this);
          }
        }
      }
    }

    /// <summary>Slices.</summary>
    /// <param name="m">Input Matrix.</param>
    /// <param name="indices">The indices.</param>
    /// <returns>A Matrix.</returns>
    public static Matrix Slice(Matrix m, IEnumerable<int> indices)
    {
      return m.Slice(indices, VectorType.Row);
    }

    /// <summary>Slices.</summary>
    /// <param name="m">Input Matrix.</param>
    /// <param name="indices">The indices.</param>
    /// <param name="t">Row or Column sum.</param>
    /// <returns>A Matrix.</returns>
    public static Matrix Slice(Matrix m, IEnumerable<int> indices, VectorType t)
    {
      var q = indices.Distinct();

      var rows = t == VectorType.Row ? q.Count(j => j < m.Rows) : m.Rows;
      var cols = t == VectorType.Col ? q.Count(j => j < m.Cols) : m.Cols;

      var n = new Matrix(rows, cols);

      var i = -1;
      foreach (var j in q.OrderBy(k => k))
        n[++i, t] = m[j, t];

      return n;
    }

    /// <summary>
    ///   Sorts the given Matrix by the specified row or column selector and returns the new Matrix
    /// </summary>
    /// <param name="source">The Matrix</param>
    /// <param name="keySelector">Property selector to sort by.</param>
    /// <param name="t">Specifies whether to sort horizontally or vertically.</param>
    /// <param name="ascending">Determines whether to sort ascending or descending (Default: True)</param>
    /// <returns>New Matrix and Vector of original indices.</returns>
    public static Matrix Sort(Matrix source, Func<Vector, double> keySelector, VectorType t, bool ascending = true)
    {
      Vector v;
      return Sort(source, keySelector, t, ascending, out v);
    }

    /// <summary>
    ///   Sorts the given Matrix by the specified row or column selector and returns the new Matrix
    ///   along with the original indices.
    /// </summary>
    /// <param name="source">The Matrix</param>
    /// <param name="keySelector">Property selector to sort by.</param>
    /// <param name="t">Specifies whether to sort horizontally or vertically.</param>
    /// <param name="ascending">Determines whether to sort ascending or descending (Default: True)</param>
    /// <param name="indices">Vector of <paramref name="t" /> indices in the original Matrix before the sort operation.</param>
    /// <returns>New Matrix and Vector of original indices.</returns>
    public static Matrix Sort(
      Matrix source,
      Func<Vector, double> keySelector,
      VectorType t,
      bool ascending,
      out Vector indices)
    {
      var max = t == VectorType.Row ? source.Rows : source.Cols;
      Vector.Zeros(max);
      
      var arrays = t == VectorType.Row ? source.GetRows() : source.GetCols();

      var sort = (ascending
                    ? arrays.Select((i, v) => new KeyValuePair<Vector, int>(i, v))
                            .OrderBy(o => keySelector(o.Key))
                    : arrays.Select((i, v) => new KeyValuePair<Vector, int>(i, v))
                            .OrderByDescending(o => keySelector(o.Key))).ToArray();

      indices = sort.Select(s => s.Value).ToVector();

      return sort.Select(s => s.Key).ToMatrix(t);
    }

    //---------------- structural
    /// <summary>Stack a set of vectors into a matrix.</summary>
    /// <exception cref="InvalidOperationException">Thrown when the requested operation is invalid.</exception>
    /// <param name="type">.</param>
    /// <param name="vectors">.</param>
    /// <returns>A Matrix.</returns>
    internal static Matrix Stack(VectorType type, params Vector[] vectors)
    {
      if (vectors.Length == 0)
        throw new InvalidOperationException("Cannot construct Matrix from empty vector set!");

      if (vectors.Any(v => v.Length != vectors[0].Length))
        throw new InvalidOperationException("Vectors must all be of the same length!");

      var n = type == VectorType.Row ? vectors.Length : vectors[0].Length;
      var d = type == VectorType.Row ? vectors[0].Length : vectors.Length;

      var m = Zeros(n, d);
      for (var i = 0; i < vectors.Length; i++)
        m[i, type] = vectors[i];

      return m;
    }

    /// <summary>Stack a set of vectors into a matrix.</summary>
    /// <param name="vectors">.</param>
    /// <returns>A Matrix.</returns>
    public static Matrix Stack(params Vector[] vectors)
    {
      return Stack(VectorType.Row, vectors);
    }

    /// <summary>Stack a set of vectors into a matrix.</summary>
    /// <exception cref="InvalidOperationException">Thrown when the requested operation is invalid.</exception>
    /// <param name="m">Input Matrix.</param>
    /// <param name="t">Row or Column sum.</param>
    /// <returns>A Matrix.</returns>
    public static Matrix Stack(Matrix m, Matrix t)
    {
      if (m.Cols != t.Cols)
        throw new InvalidOperationException("Invalid dimension for stack operation!");

      var p = new Matrix(m.Rows + t.Rows, t.Cols);
      for (var i = 0; i < p.Rows; i++)
      for (var j = 0; j < p.Cols; j++)
        if (i < m.Rows)
          p[i, j] = m[i, j];
        else
          p[i, j] = t[i - m.Rows, j];

      return p;
    }

    /// <summary>Statistics.</summary>
    /// <param name="x">Matrix x.</param>
    /// <param name="t">(Optional) Row or Column sum.</param>
    /// <returns>A Matrix[].</returns>
    public static Matrix[] Stats(Matrix x, VectorType t = VectorType.Row)
    {
      var length = t == VectorType.Row ? x.Cols : x.Rows;
      var type = t == VectorType.Row ? VectorType.Col : VectorType.Row;
      var result = new Matrix[length];
      for (var i = 0; i < length; i++)
        result[i] = x[i, type].Stats();
      return result;
    }

    /// <summary>
    ///   Computes the standard deviation of the given matrix
    /// </summary>
    /// <param name="source"></param>
    /// <param name="t">Use column or row (default: Col)</param>
    /// <returns></returns>
    public static Vector StdDev(Matrix source, VectorType t = VectorType.Col)
    {
      var count = t == VectorType.Row ? source.Cols : source.Rows;
      var type = t == VectorType.Row ? VectorType.Col : VectorType.Row;
      var v = new Vector(count);
      for (var i = 0; i < count; i++)
        v[i] = source[i, type].StdDev();
      return v;
    }

    /// <summary>Computes the sum of every element of the matrix.</summary>
    /// <param name="m">Input Matrix.</param>
    /// <returns>sum.</returns>
    public static double Sum(Matrix m)
    {
      double sum = 0;
      for (var i = 0; i < m.Rows; i++)
      for (var j = 0; j < m.Cols; j++)
        sum += m[i, j];
      return sum;
    }

    /// <summary>
    ///   Computes the sum of either the rows or columns of a matrix and returns a vector.
    /// </summary>
    /// <param name="m">Input Matrix.</param>
    /// <param name="t">Row or Column sum.</param>
    /// <returns>Vector Sum.</returns>
    public static Vector Sum(Matrix m, VectorType t)
    {
      if (t == VectorType.Row)
      {
        var result = new Vector(m.Cols);
        for (var i = 0; i < m.Cols; i++)
        for (var j = 0; j < m.Rows; j++)
          result[i] += m[j, i];
        return result;
      }
      else
      {
        var result = new Vector(m.Rows);
        for (var i = 0; i < m.Rows; i++)
        for (var j = 0; j < m.Cols; j++)
          result[i] += m[i, j];
        return result;
      }
    }

    /// <summary>Computes the sum of every element of the matrix.</summary>
    /// <param name="m">Input Matrix.</param>
    /// <param name="i">Zero-based index of the.</param>
    /// <param name="t">Row or Column sum.</param>
    /// <returns>sum.</returns>
    public static double Sum(Matrix m, int i, VectorType t)
    {
      return m[i, t].Sum();
    }

    /// <summary>Singular Value Decomposition.</summary>
    /// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
    /// <param name="A">Input Matrix.</param>
    /// <returns>Tuple(Matrix U, Vector S, Matrix V)</returns>
    public static Tuple<Matrix, Vector, Matrix> SVD(Matrix A)
    {
      throw new NotImplementedException();
    }

    /// <summary>Swaps.</summary>
    /// <param name="from">Source for the.</param>
    /// <param name="to">to.</param>
    /// <param name="t">.</param>
    public void Swap(int from, int to, VectorType t)
    {
      var temp = this[from, t].Copy();
      this[from, t] = this[to, t];
      this[to, t] = temp;
    }

    /// <summary>Swap col.</summary>
    /// <param name="from">Source for the.</param>
    /// <param name="to">to.</param>
    public void SwapCol(int from, int to)
    {
      Swap(from, to, VectorType.Col);
    }

    //--------------- aggregation/structural
    /// <summary>Swap row.</summary>
    /// <param name="from">Source for the.</param>
    /// <param name="to">to.</param>
    public void SwapRow(int from, int to)
    {
      Swap(from, to, VectorType.Row);
    }

    /// <summary>
    ///   Performs a deep copy of the underlying matrix and returns a 2D array.
    /// </summary>
    /// <returns></returns>
    private double[][] ToArray()
    {
      if (_asTransposeRef)
        return ToTransposeArray();

      return _matrix.Select(s => s.ToArray()).ToArray();
    }

    /// <summary>Returns a string that represents the current object.</summary>
    /// <returns>A string that represents the current object.</returns>
    public override string ToString()
    {
      var maxlpad = int.MinValue;
      for (var i = 0; i < Rows; i++)
      for (var j = 0; j < Cols; j++)
      {
        var lpart = this[i, j].ToString("F6");
        if (lpart.Length > maxlpad)
          maxlpad = lpart.Length;
      }
      var matrix = new StringBuilder();
      matrix.Append("\n[");
      for (var i = 0; i < Rows; i++)
      {
        matrix.Append(i == 0 ? "[ " : " [ ");

        for (var j = 0; j < Cols; j++)
        {
          matrix.Append(" ");
          matrix.Append(this[i, j].ToString("F6", CultureInfo.InvariantCulture).PadLeft(maxlpad));
          if (j < Cols - 1)
            matrix.Append(",");
        }

        matrix.Append(i < Rows - 1 ? "],\n" : "]]");
      }

      return matrix.ToString();
    }

    /// <summary>
    ///   Performs a deep copy of the underlying matrix, transpose it and returns a 2D array.
    /// </summary>
    /// <returns></returns>
    private double[][] ToTransposeArray()
    {
      var rows = _asTransposeRef ? Rows : Cols;
      var cols = _asTransposeRef ? Cols : Rows;
      var matrix = new double[rows][];
      for (var i = 0; i < rows; i++)
      {
        matrix[i] = new double[cols];
        for (var j = 0; j < cols; j++)
          matrix[i][j] = _matrix[j][i];
      }
      return matrix;
    }

    /// <summary>Converts this object to a vector.</summary>
    /// <exception cref="InvalidOperationException">Thrown when the requested operation is invalid.</exception>
    /// <returns>This object as a Vector.</returns>
    public Vector ToVector()
    {
      if (Rows == 1)
        return this[0, VectorType.Row].Copy();

      if (Cols == 1)
        return this[0, VectorType.Col].Copy();

      throw new InvalidOperationException("Matrix conversion failed: More then one row or one column!");
    }

    /// <summary>Computes the trace of a matrix.</summary>
    /// <param name="m">Input Matrix.</param>
    /// <returns>trace.</returns>
    public static double Trace(Matrix m)
    {
      double t = 0;
      for (var i = 0; i < m.Rows && i < m.Cols; i++)
        t += m[i, i];
      return t;
    }

    /// <summary>Deep copy transpose.</summary>
    /// <returns>Matrix.</returns>
    public Matrix Transpose()
    {
      return ToTransposeArray();
    }

    /// <summary>
    ///   Unshapes the given Matrix into a Vector form along the <paramref name="dimensionType" /> axis.
    ///   <para>
    ///     Reads from the source Matrix and stacks from right to left when <paramref name="dimensionType" /> equals 'Col'
    ///     otherwise uses a bottom up approach.
    ///   </para>
    /// </summary>
    /// <param name="m">The Matrix to act on.</param>
    /// <param name="dimensionType">Type of the dimension to use when unrolling the Matrix.</param>
    /// <returns>Matrix.</returns>
    public static Vector Unshape(Matrix m, VectorType dimensionType = VectorType.Col)
    {
      return Vector.Combine(dimensionType == VectorType.Col ? m.GetCols().ToArray() : m.GetRows().ToArray());
    }

    /// <summary>Stacks.</summary>
    /// <param name="vectors">.</param>
    /// <returns>A Matrix.</returns>
    public static Matrix VStack(params Vector[] vectors)
    {
      return Stack(VectorType.Col, vectors);
    }

    /// <summary>Stacks.</summary>
    /// <exception cref="InvalidOperationException">Thrown when the requested operation is invalid.</exception>
    /// <param name="m">Input Matrix.</param>
    /// <param name="t">Row or Column sum.</param>
    /// <returns>A Matrix.</returns>
    public static Matrix VStack(Matrix m, Matrix t)
    {
      if (m.Rows != t.Rows)
        throw new InvalidOperationException("Invalid dimension for stack operation!");

      var p = new Matrix(m.Rows, m.Cols + t.Cols);
      for (var i = 0; i < p.Rows; i++)
      for (var j = 0; j < p.Cols; j++)
        if (j < m.Cols)
          p[i, j] = m[i, j];
        else
          p[i, j] = t[i, j - m.Cols];

      return p;
    }

    //--------------- creation
    /// <summary>Initial Zero Matrix (n by n)</summary>
    /// <param name="n">Size.</param>
    /// <returns>Matrix.</returns>
    public static Matrix Zeros(int n)
    {
      return new Matrix(n, n);
    }

    /// <summary>Initial zero matrix.</summary>
    /// <param name="n">.</param>
    /// <param name="d">.</param>
    /// <returns>A Matrix.</returns>
    public static Matrix Zeros(int n, int d)
    {
      return new Matrix(n, d);
    }

    #region Logical Selectors

    /// <summary>
    ///   Returns an array of index (i, j) pairs matching indices that are equal to the supplied value.
    /// </summary>
    /// <param name="mat">Matrix.</param>
    /// <param name="val">Value.</param>
    /// <returns></returns>
    public static IEnumerable<int[]> operator ==(Matrix mat, double val)
    {
      for (var i = 0; i < mat.Rows; i++)
      for (var j = 0; j < mat.Cols; j++)
        if (mat[i, j] == val)
          yield return new[] {i, j};
    }

    /// <summary>
    ///   Returns an array of index (i, j) pairs matching indices that are not equal to the supplied value.
    /// </summary>
    /// <param name="mat">Matrix.</param>
    /// <param name="val">Value.</param>
    /// <returns></returns>
    public static IEnumerable<int[]> operator !=(Matrix mat, double val)
    {
      for (var i = 0; i < mat.Rows; i++)
      for (var j = 0; j < mat.Cols; j++)
        if (mat[i, j] != val)
          yield return new[] {i, j};
    }

    /// <summary>
    ///   Returns an array of index (i, j) pairs matching indices that are less than the supplied value.
    /// </summary>
    /// <param name="mat">Matrix.</param>
    /// <param name="val">Value.</param>
    /// <returns></returns>
    public static IEnumerable<int[]> operator <(Matrix mat, double val)
    {
      for (var i = 0; i < mat.Rows; i++)
      for (var j = 0; j < mat.Cols; j++)
        if (mat[i, j] < val)
          yield return new[] {i, j};
    }

    /// <summary>
    ///   Returns an array of index (i, j) pairs matching indices that are greater than the supplied value.
    /// </summary>
    /// <param name="mat">Matrix.</param>
    /// <param name="val">Value.</param>
    /// <returns></returns>
    public static IEnumerable<int[]> operator >(Matrix mat, double val)
    {
      for (var i = 0; i < mat.Rows; i++)
      for (var j = 0; j < mat.Cols; j++)
        if (mat[i, j] > val)
          yield return new[] {i, j};
    }

    /// <summary>
    ///   Returns an array of index (i, j) pairs matching indices that are less than or equal to the supplied value.
    /// </summary>
    /// <param name="mat">Matrix.</param>
    /// <param name="val">Value.</param>
    /// <returns></returns>
    public static IEnumerable<int[]> operator <=(Matrix mat, double val)
    {
      for (var i = 0; i < mat.Rows; i++)
      for (var j = 0; j < mat.Cols; j++)
        if (mat[i, j] <= val)
          yield return new[] {i, j};
    }

    /// <summary>
    ///   Returns an array of index (i, j) pairs matching indices that are greater than or equal to the supplied value.
    /// </summary>
    /// <param name="mat">Matrix.</param>
    /// <param name="val">Value.</param>
    /// <returns></returns>
    public static IEnumerable<int[]> operator >=(Matrix mat, double val)
    {
      for (var i = 0; i < mat.Rows; i++)
      for (var j = 0; j < mat.Cols; j++)
        if (mat[i, j] >= val)
          yield return new[] {i, j};
    }

    #endregion
  }
}