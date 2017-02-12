using numl.Model;

namespace numl.Tests.UnsupervisedTests
{
  public class AB
  {
    [Feature]
    public double A { get; set; }

    [Feature]
    public double B { get; set; }

    public override string ToString() { return string.Format("[{0}, {1}]", A, B); }
  }
}