﻿using Xunit;

namespace numl.Tests.ReinforcementTests
{
  [Trait("Category", "Reinforcement")]
  public class MDPTests
  {
    [Fact]
    public void Test_Convert_Action_Vectors()
    {
      Vector tests = new Vector(
        new[]
        {
          1,
          1.0,
          10,
          10.0,
          10.01,
          101.12,
          102.243,
          103.3433,
          1004.342231,
          1004.3424100,
          -99.9432,
          -8.019
        });

      int[] actuals =
      {
        1,
        1,
        10,
        10,
        1001,
        10112,
        102243,
        1033433,
        1004342231,
        100434241,
        -999432,
        -8019
      };

      for (var x = 0; x < tests.Length; x++)
        Assert.Equal(actuals[x], numl.AI.Action.GetActionId(tests[x]));
    }

    [Fact]
    public void Test_Convert_MDPStates()
    {
      var states = new Matrix(
        new[]
        {
          new[] {1.0, 0.50, 0.75, 0.82},
          new[] {2.0, 0.71, 0.71, 0.82},
          new[] {3.0, 0.63, 0.69, 0.83},
          new[] {2.0, 0.56, 0.73, 0.82},
          new[] {4.0, 0.67, 0.74, 0.81},
          new[] {5.0, 0.73, 0.71, 0.82},
          new[] {4.0, 0.67, 0.74, 0.81},
          new[] {4.0, 0.67, 0.74, 0.81}
        });

      var actions = new Vector(
        new[]
        {
          1.0,
          1.0,
          1.0,
          2.0,
          1.0,
          0.0,
          0.0,
          0.0
        });

      var statesP = new Matrix(
        new[]
        {
          new[] {2.0, 0.50, 0.75, 0.82}, // 1 > 2
          new[] {3.0, 0.71, 0.71, 0.82}, // 2 > 3
          new[] {4.0, 0.63, 0.69, 0.83}, // 3 > 4
          new[] {5.0, 0.56, 0.73, 0.82}, // 2 > 5
          new[] {5.0, 0.67, 0.74, 0.81}, // 4 > 5
          new[] {2.0, 0.73, 0.71, 0.82}, // 5 > 2
          new[] {2.0, 0.71, 0.71, 0.82}, // 4 > 2
          new[] {2.0, 0.71, 0.71, 0.82} // 4 > 2 (duplicate)
        });

      var rewards = new Vector(
        new[]
        {
          0.01,
          0.02,
          0.02,
          0.03,
          0.01,
          0.01,
          0.03,
          0.03
        });

      numl.Math.Summary summary = numl.Math.Summary.Summarize(states);

      var mdps = numl.Reinforcement.MDPConverter.GetStates(
        states,
        actions,
        statesP,
        rewards,
        null,
        new BinningDiscretizer(new Vector(new double[] {5, 1, 1, 1})));

      Assert.True(mdps.Count() == 1, "Length of starting states was more than one");
    }
  }
}