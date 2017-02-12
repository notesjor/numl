﻿using System;
using System.Linq;
using Xunit;
using numl.Math.Metrics;

namespace numl.Tests.MathTests
{
  [Trait("Category", "Math")]
  public class MetricTests
  {
    [Theory]
    [InlineData(new[]{ 1d, 5, 2, 3, 10}, new[]{ 4d, 15, 20, 5, 5}, typeof(CosineDistance), 0.40629)]
    [InlineData(new[]{ 1d, 2, 3}, new[]{ 2d, 4, 6}, typeof(EuclidianDistance), 3.7416573867739413855837487323165)]
    [InlineData(new[]{ 1d, 0, 0, 1, 1}, new[]{ 0d, 0, 1, 0, 1}, typeof(HammingDistance), 3)]
    public void Distance_Test(double[] x, double[] y, Type t, double truth)
    {
      Assert.True(t.GetInterfaces().Contains(typeof(IDistance)));

      var distance = (IDistance) Activator.CreateInstance(t);
      var result = distance.Compute(new Vector(x), new Vector(y));

      truth = Math.Round(truth, 4);
      result = Math.Round(result, 4);

      Assert.Equal(truth, result);
    }
  }
}