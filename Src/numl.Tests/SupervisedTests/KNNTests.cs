using Xunit;

namespace numl.Tests.SupervisedTests
{
  [Trait("Category", "Supervised")]
  public class KNNTests : BaseSupervised
  {
    [Fact]
    public void House_Tests() { HousePrediction(new KNNGenerator()); }

    [Fact]
    public void Iris_Learner_Tests() { IrisLearnerPrediction(new KNNGenerator()); }

    [Fact]
    public void Iris_Tests() { IrisPrediction(new KNNGenerator()); }

    [Fact]
    public void Tennis_Learner_Tests() { TennisLearnerPrediction(new KNNGenerator()); }

    [Fact]
    public void Tennis_Tests() { TennisPrediction(new KNNGenerator()); }
  }
}