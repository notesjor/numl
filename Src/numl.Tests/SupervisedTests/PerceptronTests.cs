﻿using Xunit;

namespace numl.Tests.SupervisedTests
{
  [Trait("Category", "Supervised")]
  public class PerceptronTests
  {
    [Fact]
    public void Test_Perceptron_Simple()
    {
      PerceptronGenerator generator = new PerceptronGenerator();
      Matrix x = new[,]
      {
        {1, 0, 0},
        {1, 0, 1},
        {1, 1, 0},
        {1, 1, 1}
      };

      var test = x.Copy();

      Vector y = new[] {1, 1, -1, -1};

      var model = generator.Generate(x, y);

      Vector z = Vector.Zeros(4);

      for (var i = 0; i < test.Rows; i++)
        z[i] = model.Predict((Vector) test.Row(i)) <= 0 ? -1 : 1;

      Assert.Equal(y, z);
    }

    [Fact]
    public void Test_Perceptron_Simple_2()
    {
      PerceptronGenerator generator = new PerceptronGenerator();
      Matrix x = new[,]
      {
        {1, 4}, // yes
        {-1, 3}, // no
        {-1, 2}, // no
        {-1, 1}, // no
        {-2, 1}, // no
        {-2, 2}, // no
        {2, 3}, // yes
        {3, 2}, // yes
        {3, 3}, // yes
        {4, 2}, // yes
        {4, 1} // yes
      };

      var test = x.Copy();

      Vector y = new[] {1, -1, -1, -1, -1, -1, 1, 1, 1, 1, 1};

      var model = generator.Generate(x, y);

      Vector z = Vector.Zeros(11);

      for (var i = 0; i < test.Rows; i++)
        z[i] = model.Predict((Vector) test.Row(i)) <= 0 ? -1 : 1;

      Assert.Equal(y, z);
    }
  }
}