using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using numl.Math.Probability;

namespace numl.Utils
{
  /// <summary>
  ///   Extension methods for IEnumerable collections
  /// </summary>
  public static class EnumerableHelpers
  {
    /// <summary>
    ///   Executes a batch operation on the source collection, optionally running in parallel.
    /// </summary>
    /// <typeparam name="T">Type.</typeparam>
    /// <param name="source">Enumerable to batch.</param>
    /// <param name="batchSize">Size of each batch.</param>
    /// <param name="batchCommand">Action to apply on each batch, with batch index and corresponding items.</param>
    /// <param name="asParallel">If <c>True</c>, each batch is run asynchronously.</param>
    /// <param name="threads">Number of worker threads.</param>
    public static void Batch<T>(
      this IEnumerable<T> source,
      int batchSize,
      Action<int, IEnumerable<T>> batchCommand,
      bool asParallel = false,
      int threads = 1)
    {
      var count = source.Count();
      var batches = (int) System.Math.Ceiling(count / (double) batchSize);

      if (count > 0)
        if (!asParallel)
        {
          for (var batch = 0; batch < batches; batch++)
          {
            var bin = source.Skip(batch * batchSize).Take(batchSize);
            batchCommand(batch, bin);
          }
        }
        else
        {
          var partitioner = Partitioner.Create(0, count, (int) System.Math.Floor(batchSize / (double) threads));
          var batch = -1;
          Parallel.ForEach(
            partitioner,
            (range, loopState) =>
            {
              var bin = new T[range.Item2 - 1 - range.Item2];
              for (var i = range.Item1; i < range.Item2; i++)
                bin[i] = source.ElementAt(i);

              batchCommand(batch++, bin);
            });
        }
    }

    /// <summary>
    ///   Performs the specified action and returns the result.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="R"></typeparam>
    /// <param name="source">Source array</param>
    /// <param name="fnTransform">Function to apply to each element</param>
    /// <returns></returns>
    public static IEnumerable<R> ForEach<T, R>(this IEnumerable<T> source, Func<T, R> fnTransform)
    {
      foreach (var t in source)
        yield return fnTransform(t);
    }

    /// <summary>
    ///   Returns the first element in a sequence applying the transform function.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="R"></typeparam>
    /// <param name="source">Source.</param>
    /// <param name="fnSelector">Selector.</param>
    /// <returns></returns>
    public static R Head<T, R>(this IEnumerable<T> source, Func<T, R> fnSelector)
    {
      return fnSelector(source.First());
    }

    /// <summary>
    ///   Returns the first element in a sequence applying the transform function.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="R"></typeparam>
    /// <param name="source">Source.</param>
    /// <param name="count">Number of items from the top to return.</param>
    /// <param name="fnSelector">Selector.</param>
    /// <returns></returns>
    public static IEnumerable<R> Head<T, R>(IEnumerable<T> source, int count, Func<T, R> fnSelector)
    {
      return source.Take(count).Select(fnSelector);
    }


    /// <summary>
    ///   Gets the index of the specified item in the source array.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source">Source array</param>
    /// <param name="item">Object to test for.</param>
    /// <returns></returns>
    public static int IndexOf<T>(this IEnumerable<T> source, T item)
    {
      var index = -1;

      for (var i = 0; i < source.Count(); i++)
        if (source.ElementAt(i).Equals(item))
        {
          index = i;
          break;
        }

      return index;
    }

    /// <summary>
    ///   Gets the index of the specified item in the source array using the specified test function.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source">Source array</param>
    /// <param name="fnPredicate">Predicate to test for.</param>
    /// <returns>Int.</returns>
    public static int IndexOf<T>(this IEnumerable<T> source, Func<T, bool> fnPredicate)
    {
      var index = -1;

      for (var i = 0; i < source.Count(); i++)
        if (fnPredicate(source.ElementAt(i)))
        {
          index = i;
          break;
        }

      return index;
    }

    /// <summary>
    ///   Calculates the slope of the source array and returns True if the values are increasing.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static bool IsAscending(this IEnumerable<double> source)
    {
      return source.ElementAt(0) - source.ElementAt(source.Count() - 1) < 0;
    }

    /// <summary>
    ///   Returns a random element from the sequence.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source">Source sequence.</param>
    /// <returns></returns>
    public static T Random<T>(this IEnumerable<T> source)
    {
      return source.ElementAt(Sampling.GetUniform(0, source.Count() - 1));
    }

    /// <summary>
    ///   Randomly shuffles the indexing of the source array.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source">Source sequence.</param>
    /// <returns></returns>
    public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
    {
      return source.OrderBy(o => Sampling.GetUniform());
    }

    /// <summary>
    ///   Returns the slice of objects using the specified index arrays.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="indices"></param>
    /// <returns></returns>
    public static IEnumerable<T> Slice<T>(this IEnumerable<T> source, IEnumerable<int> indices)
    {
      foreach (var index in indices)
        yield return source.ElementAt(index);
    }

    /// <summary>
    ///   Calculates the standard deviation on the source collection
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <param name="source"></param>
    /// <param name="fnPropSelector"></param>
    /// <param name="isSamplePopulation"></param>
    /// <returns></returns>
    public static double StandardDeviation<TSource>(
      this IEnumerable<TSource> source,
      Func<TSource, double> fnPropSelector,
      bool isSamplePopulation = false)
    {
      return System.Math.Sqrt(source.Variance(fnPropSelector, isSamplePopulation));
    }

    /// <summary>
    ///   Returns the last element in a sequence applying the transform function.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="R"></typeparam>
    /// <param name="source">Source.</param>
    /// <param name="fnSelector">Selector.</param>
    /// <returns></returns>
    public static R Tail<T, R>(this IEnumerable<T> source, Func<T, R> fnSelector)
    {
      return fnSelector(source.Last());
    }

    /// <summary>
    ///   Returns the last elements in a sequence applying the transform function.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="R"></typeparam>
    /// <param name="source">Source.</param>
    /// <param name="count">Number of items from the end to return.</param>
    /// <param name="fnSelector">Selector.</param>
    /// <returns></returns>
    public static IEnumerable<R> Tail<T, R>(this IEnumerable<T> source, int count, Func<T, R> fnSelector)
    {
      return source.Reverse().Take(count).Select(fnSelector);
    }

    /// <summary>
    ///   Calculates the variance on the source collection
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <param name="source"></param>
    /// <param name="fnPropSelector"></param>
    /// <param name="isSamplePopulation"></param>
    /// <returns></returns>
    public static double Variance<TSource>(
      this IEnumerable<TSource> source,
      Func<TSource, double> fnPropSelector,
      bool isSamplePopulation = false)
    {
      return source.Select(s => System.Math.Pow(fnPropSelector(s) - source.Average(fnPropSelector), 2)).Sum()
             / (isSamplePopulation ? source.Count() - 1 : source.Count());
    }
  }
}