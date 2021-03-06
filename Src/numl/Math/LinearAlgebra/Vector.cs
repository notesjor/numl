﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using numl.Math.Probability;
using numl.Serialization;

namespace numl.Math.LinearAlgebra
{
  /// <summary>A vector.</summary>
  public class Vector : IEnumerable<double>
  {
    /// <summary>The empty.</summary>
    public static readonly Vector Empty = new[] {0};

    /// <summary>true to as col.</summary>
    private readonly bool _asCol;

    /// <summary>The matrix.</summary>
    private readonly double[][] _matrix;

    /// <summary>true to as matrix reference.</summary>
    private readonly bool _asMatrixRef;

    /// <summary>Zero-based index of the static.</summary>
    private readonly int _staticIdx = -1;

    /// <summary>The transpose.</summary>
    private Matrix _transpose;

    /// <summary>The vector.</summary>
    private readonly double[] _vector;

    /// <summary>
    ///   this is when the values are actually referencing a vector in an existing matrix.
    /// </summary>
    /// <param name="m">private matrix vals.</param>
    /// <param name="idx">static col reference.</param>
    /// <param name="asCol">(Optional) true to as col.</param>
    internal Vector(double[][] m, int idx, bool asCol = false)
    {
      _asCol = asCol;
      _asMatrixRef = true;
      _matrix = m;
      _staticIdx = idx;
    }

    /// <summary>
    ///   Constructor that prevents a default instance of this class from being created.
    /// </summary>
    private Vector()
    {
      _asCol = false;
      _asMatrixRef = false;
    }

    /// <summary>
    ///   this is when the values are actually referencing a vector in an existing matrix.
    /// </summary>
    /// <param name="n">The int to process.</param>
    public Vector(int n)
    {
      _asCol = false;
      _asMatrixRef = false;
      _vector = new double[n];
    }

    /// <summary>
    ///   this is when the values are actually referencing a vector in an existing matrix.
    /// </summary>
    /// <param name="contents">The contents.</param>
    public Vector(IEnumerable<double> contents) : this(contents.ToArray()) {}

    /// <summary>
    ///   this is when the values are actually referencing a vector in an existing matrix.
    /// </summary>
    /// <param name="contents">The contents.</param>
    public Vector(double[] contents)
    {
      _asCol = false;
      _asMatrixRef = false;
      _vector = contents;
    }

    /// <summary>Indexer to set items within this collection using array index syntax.</summary>
    /// <param name="f">The Predicate&lt;double&gt; to process.</param>
    /// <returns>The indexed item.</returns>
    public double this[Predicate<double> f]
    {
      set
      {
        for (var i = 0; i < Length; i++)
          if (f(this[i]))
            this[i] = value;
      }
    }

    /// <summary>Indexer to set items within this collection using array index syntax.</summary>
    /// <param name="slice">The slice.</param>
    /// <returns>The indexed item.</returns>
    public double this[IEnumerable<int> slice]
    {
      set
      {
        foreach (var i in slice)
          this[i] = value;
      }
    }

    /// <summary>
    ///   Indexer to get or set items within this collection using array index syntax.
    /// </summary>
    /// <param name="i">Zero-based index of the entry to access.</param>
    /// <returns>The indexed item.</returns>
    public double this[int i]
    {
      get
      {
        if (!_asMatrixRef)
          return _vector[i];
        if (_asCol)
          return _matrix[_staticIdx][i];
        return _matrix[i][_staticIdx];
      }
      set
      {
        if (!_asMatrixRef)
        {
          _vector[i] = value;
        }
        else
        {
          if (_asCol)
            _matrix[_staticIdx][i] = value;
          else
            _matrix[i][_staticIdx] = value;
        }
      }
    }

    /// <summary>Gets the length.</summary>
    /// <value>The length.</value>
    public int Length
    {
      get
      {
        if (!_asMatrixRef)
          return _vector.Length;
        if (_asCol)
          return _matrix[0].Length;
        return _matrix.Length;
      }
    }

    /// <summary>Gets the t.</summary>
    /// <value>The t.</value>
    public Matrix T
    {
      get
      {
        if (_transpose == null)
          _transpose = new Matrix(Length, 1);
        _transpose[0, VectorType.Col] = this;
        return _transpose;
      }
    }

    /// <summary>Gets the enumerator.</summary>
    /// <returns>The enumerator.</returns>
    IEnumerator IEnumerable.GetEnumerator()
    {
      for (var i = 0; i < Length; i++)
        yield return this[i];
    }

    /// <summary>Gets the enumerator.</summary>
    /// <returns>The enumerator.</returns>
    public IEnumerator<double> GetEnumerator()
    {
      for (var i = 0; i < Length; i++)
        yield return this[i];
    }

    /// <summary>Calcs.</summary>
    /// <param name="v">A variable-length parameters list containing v.</param>
    /// <param name="f">The Func&lt;int,double,double&gt; to process.</param>
    /// <returns>A Vector.</returns>
    public static Vector Calc(Vector v, Func<double, double> f)
    {
      var result = v.ToArray();
      for (var i = 0; i < v.Length; i++)
        result[i] = f(result[i]);
      return result;
    }

    /// <summary>Calcs.</summary>
    /// <param name="v">A variable-length parameters list containing v.</param>
    /// <param name="f">The Func&lt;int,double,double&gt; to process.</param>
    /// <returns>A Vector.</returns>
    public static Vector Calc(Vector v, Func<int, double, double> f)
    {
      var result = v.ToArray();
      for (var i = 0; i < v.Length; i++)
        result[i] = f(i, result[i]);
      return result;
    }

    /// <summary>Combines the given v.</summary>
    /// <exception cref="InvalidOperationException">Thrown when the requested operation is invalid.</exception>
    /// <param name="v">A variable-length parameters list containing v.</param>
    /// <returns>A Vector.</returns>
    public static Vector Combine(params Vector[] v)
    {
      if (v.Length == 0)
        throw new InvalidOperationException("Need to specify vectors to combine!");

      if (v.Length == 1)
        return v[0];

      var size = 0;
      for (var i = 0; i < v.Length; i++)
        size += v[i].Length;

      var r = new Vector(size);
      var z = -1;
      for (var i = 0; i < v.Length; i++)
      for (var j = 0; j < v[i].Length; j++)
        r[++z] = v[i][j];

      return r;
    }

    /// <summary>Query if 'vector' contains NaN.</summary>
    /// <param name="vector">The vector.</param>
    /// <returns>true if it succeeds, false if it fails.</returns>
    public static bool ContainsNaN(Vector vector)
    {
      var v = vector.ToArray();
      return v.Any(double.IsNaN);
    }

    /// <summary>Copies this object.</summary>
    /// <returns>A Vector.</returns>
    public Vector Copy()
    {
      return new Vector(ToArray());
    }

    /// <summary>Creates a new Vector.</summary>
    /// <param name="length">The length.</param>
    /// <param name="f">The Func&lt;int,double&gt; to process.</param>
    /// <returns>A Vector.</returns>
    public static Vector Create(int length, Func<double> f)
    {
      var vector = new double[length];
      for (var i = 0; i < length; i++)
        vector[i] = f();
      return new Vector(vector);
    }

    /// <summary>Creates a new Vector.</summary>
    /// <param name="length">The length.</param>
    /// <param name="f">The Func&lt;int,double&gt; to process.</param>
    /// <returns>A Vector.</returns>
    public static Vector Create(int length, Func<int, double> f)
    {
      var vector = new double[length];
      for (var i = 0; i < length; i++)
        vector[i] = f(i);
      return new Vector(vector);
    }

    /// <summary>Diags.</summary>
    /// <param name="v">A variable-length parameters list containing v.</param>
    /// <returns>A Matrix.</returns>
    public static Matrix Diag(Vector v)
    {
      var m = Matrix.Zeros(v.Length);
      for (var i = 0; i < v.Length; i++)
        m[i, i] = v[i];
      return m;
    }

    /// <summary>Diags.</summary>
    /// <param name="v">A variable-length parameters list containing v.</param>
    /// <param name="n">The int to process.</param>
    /// <param name="d">The int to process.</param>
    /// <returns>A Matrix.</returns>
    public static Matrix Diag(Vector v, int n, int d)
    {
      var m = Matrix.Zeros(n, d);
      var min = System.Math.Min(n, d);
      for (var i = 0; i < min; i++)
        m[i, i] = v[i];
      return m;
    }

    /// <summary>Dots.</summary>
    /// <exception cref="InvalidOperationException">Thrown when the requested operation is invalid.</exception>
    /// <param name="one">The one.</param>
    /// <param name="two">The two.</param>
    /// <returns>A double.</returns>
    public static double Dot(Vector one, Vector two)
    {
      if (one.Length != two.Length)
        throw new InvalidOperationException("Dimensions do not match!");

      double total = 0;
      for (var i = 0; i < one.Length; i++)
        total += one[i] * two[i];
      return total;
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
      if (obj is Vector)
      {
        var m = obj as Vector;
        if (Length != m.Length)
          return false;

        for (var i = 0; i < Length; i++)
          if (this[i] != m[i])
            return false;

        return true;
      }
      return false;
    }

    /// <summary>Exponents the given v.</summary>
    /// <param name="v">A variable-length parameters list containing v.</param>
    /// <returns>A Vector.</returns>
    public static Vector Exp(Vector v)
    {
      return Calc(v, System.Math.Exp);
    }

    /// <summary>Serves as a hash function for a particular type.</summary>
    /// <returns>A hash code for the current <see cref="T:System.Object" />.</returns>
    public override int GetHashCode() => _asMatrixRef ? _matrix.GetHashCode() : _vector.GetHashCode();

    /// <summary>Query if 'vector' is NaN.</summary>
    /// <param name="vector">The vector.</param>
    /// <returns>true if NaN, false if not.</returns>
    public static bool IsNaN(Vector vector)
    {
      var v = vector.ToArray();
      return v.All(double.IsNaN);
    }

    /// <summary>Loads the given vector file.</summary>
    /// <exception cref="InvalidOperationException">Thrown when the requested file is not present.</exception>
    /// <param name="file">The file to load.</param>
    /// <returns>A Vector.</returns>
    public static Vector Load(string file)
    {
      if (File.Exists(file))
        using (var fs = new FileStream(file, FileMode.Open))
        {
          using (var f = new StreamReader(fs))
          {
            using (var r = new JsonReader(f))
            {
              return r.ReadVector();
            }
          }
        }
      throw new InvalidOperationException("File not found");
    }

    /// <summary>Logs the given v.</summary>
    /// <param name="v">A variable-length parameters list containing v.</param>
    /// <returns>A Vector.</returns>
    public static Vector Log(Vector v)
    {
      return Calc(v, d => System.Math.Log(d));
    }

    /// <summary>Maximum index.</summary>
    /// <param name="startAt">The start at.</param>
    /// <returns>An int.</returns>
    internal int MaxIndex(int startAt)
    {
      var idx = startAt;
      double val = 0;
      for (var i = startAt; i < Length; i++)
        if (val < this[i])
        {
          idx = i;
          val = this[i];
        }

      return idx;
    }
    
    /// <summary>Normals.</summary>
    /// <exception cref="InvalidOperationException">Thrown when the requested operation is invalid.</exception>
    /// <param name="x">The Vector to process.</param>
    /// <param name="p">The double to process.</param>
    /// <returns>A double.</returns>
    public static double Norm(Vector x, double p = 2)
    {
      if (p < 1)
        throw new InvalidOperationException("p must be greater than 0");
      double value = 0;
      if (p == 1)
      {
        for (var i = 0; i < x.Length; i++)
          value += System.Math.Abs(x[i]);

        return value;
      }
      if (p == int.MaxValue)
      {
        for (var i = 0; i < x.Length; i++)
          if (System.Math.Abs(x[i]) > value)
            value = System.Math.Abs(x[i]);
        return value;
      }
      if (p == int.MinValue)
      {
        for (var i = 0; i < x.Length; i++)
          if (System.Math.Abs(x[i]) < value)
            value = System.Math.Abs(x[i]);
        return value;
      }
      for (var i = 0; i < x.Length; i++)
        value += System.Math.Pow(System.Math.Abs(x[i]), p);

      return System.Math.Pow(value, 1 / p);
    }

    /// <summary>Normalise random.</summary>
    /// <param name="n">The int to process.</param>
    /// <param name="mean">(Optional) the mean.</param>
    /// <param name="stdDev">(Optional) the standard development.</param>
    /// <param name="precision">(Optional) the precision.</param>
    /// <returns>A Vector.</returns>
    public static Vector NormRand(int n, double mean = 0, double stdDev = 1, int precision = -1)
    {
      var x = new double[n];
      for (var i = 0; i < n; i++)
        if (precision > -1)
          x[i] = System.Math.Round(Sampling.GetNormal(mean, stdDev), precision);
        else
          x[i] = Sampling.GetNormal(mean, stdDev);

      return new Vector(x);
    }

    /// <summary>Ones.</summary>
    /// <param name="n">The int to process.</param>
    /// <returns>A Vector.</returns>
    public static Vector Ones(int n)
    {
      var x = new double[n];
      for (var i = 0; i < n; i++)
        x[i] = 1;

      return new Vector(x);
    }

    /// <summary>Addition operator.</summary>
    /// <exception cref="InvalidOperationException">Thrown when the requested operation is invalid.</exception>
    /// <param name="one">The one.</param>
    /// <param name="two">The two.</param>
    /// <returns>The result of the operation.</returns>
    public static Vector operator +(Vector one, Vector two)
    {
      if (one.Length != two.Length)
        throw new InvalidOperationException("Dimensions do not match!");

      var result = one.ToArray();
      for (var i = 0; i < result.Length; i++)
        result[i] += two[i];

      return new Vector(result);
    }

    /// <summary>Addition operator.</summary>
    /// <param name="v">The Vector to process.</param>
    /// <param name="s">The double to process.</param>
    /// <returns>The result of the operation.</returns>
    public static Vector operator +(Vector v, double s)
    {
      var result = v.ToArray();
      for (var i = 0; i < result.Length; i++)
        result[i] += s;

      return new Vector(result);
    }

    /// <summary>Addition operator.</summary>
    /// <param name="s">The double to process.</param>
    /// <param name="v">The Vector to process.</param>
    /// <returns>The result of the operation.</returns>
    public static Vector operator +(double s, Vector v)
    {
      return v + s;
    }

    /// <summary>Division operator.</summary>
    /// <param name="one">The one.</param>
    /// <param name="two">The two.</param>
    /// <returns>The result of the operation.</returns>
    public static Vector operator /(Vector one, double two)
    {
      var result = one.ToArray();
      for (var i = 0; i < one.Length; i++)
        result[i] /= two;
      return new Vector(result);
    }

    /// <summary>Division operator.</summary>
    /// <param name="one">The one.</param>
    /// <param name="two">The two.</param>
    /// <returns>The result of the operation.</returns>
    public static Vector operator /(Vector one, int two)
    {
      var result = one.ToArray();
      for (var i = 0; i < one.Length; i++)
        result[i] /= two;

      return new Vector(result);
    }

    /// <summary>Equality operator.</summary>
    /// <param name="one">The one.</param>
    /// <param name="two">The two.</param>
    /// <returns>The result of the operation.</returns>
    public static bool operator ==(Vector one, Vector two)
    {
      return ReferenceEquals(one, null) && ReferenceEquals(two, null) || one.Equals(two);
    }

    /// <summary>Raises each value to the specified power.</summary>
    /// <param name="one">The one.</param>
    /// <param name="power">The power.</param>
    /// <returns>The result of the operation.</returns>
    public static Vector operator ^(Vector one, double power)
    {
      return one.Each(d => System.Math.Pow(d, power));
    }

    /// <summary>
    ///   Returns an array of matching indices where each value is greater than the supplied value.
    /// </summary>
    /// <param name="one"></param>
    /// <param name="val"></param>
    /// <returns></returns>
    public static IEnumerable<int> operator >(Vector one, double val)
    {
      for (var i = 0; i < one.Length; i++)
        if (one[i] > val)
          yield return i;
    }

    /// <summary>double[] casting operator.</summary>
    /// <param name="v">The Vector to process.</param>
    public static implicit operator double[](Vector v)
    {
      return v.ToArray();
    }

    /// <summary>Vector casting operator.</summary>
    /// <param name="array">The array.</param>
    public static implicit operator Vector(double[] array)
    {
      return new Vector(array);
    }

    /// <summary>Vector casting operator.</summary>
    /// <param name="array">The array.</param>
    public static implicit operator Vector(int[] array)
    {
      var vector = new double[array.Length];
      for (var i = 0; i < array.Length; i++)
        vector[i] = array[i];

      return new Vector(vector);
    }

    /// <summary>Vector casting operator.</summary>
    /// <param name="array">The array.</param>
    public static implicit operator Vector(float[] array)
    {
      var vector = new double[array.Length];
      for (var i = 0; i < array.Length; i++)
        vector[i] = array[i];

      return new Vector(vector);
    }

    /// <summary>Inequality operator.</summary>
    /// <param name="one">The one.</param>
    /// <param name="two">The two.</param>
    /// <returns>The result of the operation.</returns>
    public static bool operator !=(Vector one, Vector two)
    {
      return !one.Equals(two);
    }

    /// <summary>
    ///   Returns an array of matching indices where each value is less than the supplied value.
    /// </summary>
    /// <param name="one"></param>
    /// <param name="val"></param>
    /// <returns></returns>
    public static IEnumerable<int> operator <(Vector one, double val)
    {
      for (var i = 0; i < one.Length; i++)
        if (one[i] < val)
          yield return i;
    }

    /// <summary>
    ///   Mod operator.
    /// </summary>
    /// <param name="one">The one.</param>
    /// <param name="two">The mod.</param>
    /// <returns></returns>
    public static Vector operator %(Vector one, double two)
    {
      return one.Each(d => d % two);
    }

    /// <summary>Multiplication operator.</summary>
    /// <exception cref="InvalidOperationException">Thrown when the requested operation is invalid.</exception>
    /// <param name="one">The one.</param>
    /// <param name="two">The two.</param>
    /// <returns>The result of the operation.</returns>
    public static Vector operator *(Vector one, Vector two)
    {
      if (one.Length != two.Length)
        throw new InvalidOperationException("Dimensions do not match!");

      var result = one.ToArray();
      for (var i = 0; i < one.Length; i++)
        result[i] *= two[i];
      return new Vector(result);
    }

    /// <summary>Multiplication operator.</summary>
    /// <param name="one">The one.</param>
    /// <param name="two">The two.</param>
    /// <returns>The result of the operation.</returns>
    public static Vector operator *(Vector one, double two)
    {
      var result = one.ToArray();
      for (var i = 0; i < one.Length; i++)
        result[i] *= two;
      return new Vector(result);
    }

    /// <summary>Multiplication operator.</summary>
    /// <param name="one">The one.</param>
    /// <param name="two">The two.</param>
    /// <returns>The result of the operation.</returns>
    public static Vector operator *(Vector one, int two)
    {
      var result = one.ToArray();
      for (var i = 0; i < one.Length; i++)
        result[i] *= two;
      return new Vector(result);
    }

    /// <summary>Multiplication operator.</summary>
    /// <param name="one">The one.</param>
    /// <param name="two">The two.</param>
    /// <returns>The result of the operation.</returns>
    public static Vector operator *(double one, Vector two)
    {
      return two * one;
    }

    /// <summary>Subtraction operator.</summary>
    /// <exception cref="InvalidOperationException">Thrown when the requested operation is invalid.</exception>
    /// <param name="one">The one.</param>
    /// <param name="two">The two.</param>
    /// <returns>The result of the operation.</returns>
    public static Vector operator -(Vector one, Vector two)
    {
      if (one.Length != two.Length)
        throw new InvalidOperationException("Dimensions do not match!");

      var result = one.ToArray();
      for (var i = 0; i < result.Length; i++)
        result[i] -= two[i];

      return new Vector(result);
    }

    /// <summary>Subtraction operator.</summary>
    /// <param name="v">The Vector to process.</param>
    /// <param name="s">The double to process.</param>
    /// <returns>The result of the operation.</returns>
    public static Vector operator -(Vector v, double s)
    {
      var result = v.ToArray();
      for (var i = 0; i < result.Length; i++)
        result[i] -= s;

      return new Vector(result);
    }

    /// <summary>Subtraction operator.</summary>
    /// <param name="s">The double to process.</param>
    /// <param name="v">The Vector to process.</param>
    /// <returns>The result of the operation.</returns>
    public static Vector operator -(double s, Vector v)
    {
      var result = v.ToArray();
      for (var i = 0; i < result.Length; i++)
        result[i] = s - result[i];

      return new Vector(result);
    }

    /// <summary>Negation operator.</summary>
    /// <param name="one">The one.</param>
    /// <returns>The result of the operation.</returns>
    public static Vector operator -(Vector one)
    {
      var result = one.ToArray();
      for (var i = 0; i < result.Length; i++)
        result[i] *= -1;

      return new Vector(result);
    }

    /// <summary>Outers.</summary>
    /// <exception cref="InvalidOperationException">Thrown when the requested operation is invalid.</exception>
    /// <param name="x">The Vector to process.</param>
    /// <param name="y">The Vector to process.</param>
    /// <returns>A Matrix.</returns>
    public static Matrix Outer(Vector x, Vector y)
    {
      if (x.Length != y.Length)
        throw new InvalidOperationException("Dimensions do not match!");

      var n = x.Length;
      var m = new Matrix(n);
      for (var i = 0; i < n; i++)
      for (var j = 0; j < n; j++)
        m[i, j] = x[i] * y[j];

      return m;
    }

    /// <summary>
    ///   Parses a string containing MATLAB style Vector syntax, i.e. "[1, 2, 3];"
    /// </summary>
    /// <param name="text">Input string to parse.</param>
    /// <returns>Matrix.</returns>
    public static Vector Parse(string text)
    {
      var arrs = text.Split(new[] {'[', ',', ';', ']'}, StringSplitOptions.RemoveEmptyEntries);

      var result = new Vector(arrs.Length);

      for (var i = 0; i < result.Length; i++)
        result[i] = double.Parse(arrs[i].Trim());

      return result;
    }

    /// <summary>Products the given v.</summary>
    /// <param name="v">A variable-length parameters list containing v.</param>
    /// <returns>A double.</returns>
    public static double Prod(Vector v)
    {
      var prod = v[0];
      for (var i = 1; i < v.Length; i++)
        prod *= v[i];
      return prod;
    }

    /// <summary>Rands.</summary>
    /// <param name="n">The int to process.</param>
    /// <returns>A Vector.</returns>
    public static Vector Rand(int n)
    {
      var x = new double[n];
      for (var i = 0; i < n; i++)
        x[i] = Sampling.GetUniform();

      return new Vector(x);
    }

    /// <summary>Ranges.</summary>
    /// <param name="s">The int to process.</param>
    /// <param name="e">(Optional) the int to process.</param>
    /// <returns>A Vector.</returns>
    public static Vector Range(int s, int e = -1)
    {
      if (e > 0)
      {
        var v = Zeros(e - s);
        for (var i = s; i < e; i++)
          v[i - s] = i;
        return v;
      }
      else
      {
        var v = Zeros(s);
        for (var i = 0; i < s; i++)
          v[i] = i;
        return v;
      }
    }

    /// <summary>Rounds.</summary>
    /// <param name="v">A variable-length parameters list containing v.</param>
    /// <param name="decimals">(Optional) the decimals.</param>
    /// <returns>A Vector.</returns>
    public static Vector Round(Vector v, int decimals = 0)
    {
      for (var i = 0; i < v.Length; i++)
        v[i] = System.Math.Round(v[i], decimals);
      return v;
    }

    /// <summary>
    ///   Save vector to file
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
            w.WriteVector(this);
          }
        }
      }
    }

    /// <summary>Sort order.</summary>
    /// <param name="vector">The vector.</param>
    /// <returns>The sorted order.</returns>
    public static Vector SortOrder(Vector vector)
    {
      return vector
        .Select((d, i) => new Tuple<int, double>(i, d))
        .OrderByDescending(t => t.Item2)
        .Select(t => t.Item1)
        .ToArray();
    }

    /// <summary>Sums the given v.</summary>
    /// <param name="v">A variable-length parameters list containing v.</param>
    /// <returns>A double.</returns>
    public static double Sum(Vector v)
    {
      return v.ToArray().Sum();
    }

    /// <summary>Convert this object into an array representation.</summary>
    /// <returns>An array that represents the data in this object.</returns>
    public double[] ToArray()
    {
      var toReturn = new double[Length];
      for (var i = 0; i < Length; i++)
        toReturn[i] = this[i];
      return toReturn;
    }

    /// <summary>Converts a t to a matrix.</summary>
    /// <param name="t">The VectorType to process.</param>
    /// <returns>t as a Matrix.</returns>
    public Matrix ToMatrix(VectorType t = VectorType.Row)
    {
      if (t == VectorType.Row)
      {
        var m = new Matrix(1, Length);
        for (var j = 0; j < Length; j++)
          m[0, j] = this[j];

        return m;
      }
      else
      {
        var m = new Matrix(Length, 1);
        for (var i = 0; i < Length; i++)
          m[i, 0] = this[i];

        return m;
      }
    }

    /// <summary>Returns a string that represents the current object.</summary>
    /// <returns>A string that represents the current object.</returns>
    public override string ToString()
    {
      var sb = new StringBuilder("[");
      for (var i = 0; i < Length; i++)
      {
        sb.Append(this[i].ToString("F4"));
        if (i < Length - 1)
          sb.Append(", ");
      }
      sb.Append("]");
      return sb.ToString();
    }

    /// <summary>Zeros.</summary>
    /// <param name="n">The int to process.</param>
    /// <returns>A Vector.</returns>
    public static Vector Zeros(int n)
    {
      return new Vector(n);
    }
  }
}