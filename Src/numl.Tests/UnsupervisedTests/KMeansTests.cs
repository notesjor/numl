﻿using numl.Tests.Data;
using Xunit;
using numl.Math.LinearAlgebra;
using numl.Unsupervised;

namespace numl.Tests.UnsupervisedTests
{
  [Trait("Category", "Unsupervised")]
  public class KMeansTests
  {
    private static Matrix GenerateData(int size)
    {
      // this creates [size] points in graph quadrant 1
      var A = Matrix.Create(size, 2, () => Sampling.GetNormal());
      A[0, VectorType.Col] -= 20;
      A[1, VectorType.Col] -= 20;

      // this creates [size] points in graph quadrant 3
      var B = Matrix.Create(size, 2, () => Sampling.GetNormal());
      B[0, VectorType.Col] += 20;
      B[1, VectorType.Col] += 20;

      // stack them
      var X = A.Stack(B);
      return X;
    }

    [Theory]
    [InlineData(10)]
    [InlineData(20)]
    [InlineData(50)]
    [InlineData(100)]
    public void Test_Numerical_KMeans(int size)
    {
      Matrix X = GenerateData(size);

      KMeans model = new KMeans();
      var assignment = model.Generate(X, 2, new EuclidianDistance());
      Assert.Equal(size * 2, assignment.Length);
      var a1 = assignment.First();
      var a2 = assignment.Last();
      for (var i = 0; i < size * 2; i++)
        if (i < size)
          Assert.Equal(a1, assignment[i]);
        else
          Assert.Equal(a2, assignment[i]);
    }

    [Theory]
    [InlineData(10)]
    [InlineData(20)]
    [InlineData(50)]
    [InlineData(100)]
    public void Test_Object_KMeans(int size)
    {
      Matrix X = GenerateData(size);
      var objects = X.GetRows()
                     .Select(v => new AB {A = v[0], B = v[1]})
                     .ToArray();

      var descriptor = Descriptor.Create<AB>();

      KMeans model = new KMeans();
      var clusters = model.Generate(descriptor, objects, 2, new EuclidianDistance());
      Assert.Equal(2, clusters.Children.Length);
      Assert.Equal(size, clusters[0].Members.Length);
      Assert.Equal(size, clusters[1].Members.Length);
    }

    [Fact]
    public void Test_Feed_KMeans()
    {
      var groups = 4;
      var feeds = Feed.GetData();
      Descriptor descriptor = Descriptor.Create<Feed>();
      KMeans kmeans = new KMeans();
      kmeans.Descriptor = descriptor;

      int[] grouping = kmeans.Generate(feeds, groups, new CosineDistance());

      for (var i = 0; i < grouping.Length; i++)
        feeds[i].Cluster = grouping[i];
    }
  }
}