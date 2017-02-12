using numl.Model;

namespace numl.Tests.SerializationTests.ModelSerialization
{
  public class ModelItem
  {
    [Feature]
    public double LeftOperand { get; set; }

    [Label]
    public double Result { get; set; }

    [Feature]
    public double RightOperand { get; set; }
  }
}