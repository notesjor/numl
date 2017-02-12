using System;
using System.IO;
using System.Linq;
using numl.Tests.Data;
using Xunit;

namespace numl.Tests.SupervisedTests
{
  [Trait("Category", "Supervised")]
  public class SVMTests : BaseSupervised
  {
    //[Fact]
    public void Test_SVM_Email_Classification()
    {
      var training_Data = File.ReadAllLines("Data\\Emails\\Training_Data.txt")
                              .Select(
                                s =>
                                  s.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries)
                                   .ToVector(f => double.Parse(f.Trim()))).ToMatrix();

      var training_Labels = File.ReadAllLines("Data\\Emails\\Training_Labels.txt")
                                .Select(s => double.Parse(s.Trim())).ToVector();

      var test_Data = File.ReadAllLines("Data\\Emails\\Test_Data.txt")
                          .Select(
                            s =>
                              s.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries)
                               .ToVector(f => double.Parse(f.Trim()))).ToMatrix();

      var test_Labels = File.ReadAllLines("Data\\Emails\\Test_Labels.txt")
                            .Select(s => double.Parse(s.Trim())).ToVector();

      var generator = new Supervised.SVM.SVMGenerator
      {
        C = 0.1,
        MaxIterations = 5,
        SelectionFunction = new Supervised.SVM.Selection.RandomSetSelection()
      };

      var model = generator.Generate(training_Data, training_Labels);

      Vector predictions = new Vector(test_Labels.Count());
      for (var x = 0; x < test_Labels.Count(); x++)
        predictions[x] = model.Predict(test_Data[x]);

      var score = numl.Supervised.Score.ScorePredictions(predictions, test_Labels);

      Console.WriteLine($"SVM Model\n: {score}");
    }

    [Fact]
    public void Test_SVM_Classification()
    {
      var students = Tennis.GetData();

      var descriptor = Descriptor.Create<Tennis>();

      var model = MultiClassLearner.Learn(
        new numl.Supervised.SVM.SVMGenerator
        {
          Descriptor = descriptor,
          C = 0.01,
          SelectionFunction = new numl.Supervised.SVM.Selection.RandomSetSelection()
        },
        students,
        0.8);

      //Assert.GreaterOrEqual(model.Accuracy, 0.65d);
      Console.WriteLine($"SVM Model\n: {model.Score}");
    }

    [Fact]
    public void Test_SVM_XOR_Classification()
    {
      var xor = new[]
      {
        new {a = false, b = false, c = false},
        new {a = false, b = true, c = true},
        new {a = true, b = false, c = true},
        new {a = true, b = true, c = false},
        new {a = false, b = false, c = false},
        new {a = false, b = true, c = true},
        new {a = true, b = false, c = true},
        new {a = true, b = true, c = false},
        new {a = false, b = false, c = false},
        new {a = false, b = true, c = true},
        new {a = true, b = false, c = true},
        new {a = true, b = true, c = false},
        new {a = false, b = false, c = false},
        new {a = false, b = true, c = true},
        new {a = true, b = false, c = true},
        new {a = true, b = true, c = false},
        new {a = false, b = false, c = false},
        new {a = false, b = true, c = true},
        new {a = true, b = false, c = true},
        new {a = true, b = true, c = false}
      };

      var d = Descriptor.New("XOR")
                        .With("a").As(typeof(bool))
                        .With("b").As(typeof(bool))
                        .Learn("c").As(typeof(bool));

      var generator = new numl.Supervised.SVM.SVMGenerator
      {
        Descriptor = d,
        C = 0.1,
        KernelFunction = new Math.Kernels.PolyKernel(3)
      };
      var model = Learner.Learn(xor, 1.0, 10, generator).Model;

      Matrix x = new[,]
      {
        {-1, -1}, // false, false -> -
        {-1, 1}, // false, true  -> +
        {1, -1}, // true, false  -> +
        {1, 1}
      }; // true, true   -> -

      Vector actual = new[] {-1, 1, 1, -1};

      Vector y = new[] {0, 0, 0, 0};

      for (var i = 0; i < x.Rows; i++)
        y[i] = model.Predict(x[i, VectorType.Row]);

      var score = numl.Supervised.Score.ScorePredictions(y, actual);
      Console.WriteLine($"SVM Model\n: {score}");
    }
  }
}