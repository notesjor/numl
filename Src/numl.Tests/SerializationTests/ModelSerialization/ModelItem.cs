namespace numl.Tests.SerializationTests.ModelSerialization
{
  public class ModelItem
  {
    [Feature]
    public double LeftOperand { get; set; }
    [Feature]
    public double RightOperand { get; set; }
    [Label]
    public double Result { get; set; }
  }
}