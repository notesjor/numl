using numl.Data;

namespace numl.Tests.DataTests
{
  public class Edge : IEdge
  {
    public int ChildId { get; set; }

    public int ParentId { get; set; }

    public override bool Equals(object obj)
    {
      if (obj is Edge)
        return ((Edge) obj).ChildId == ChildId && ((Edge) obj).ParentId == ParentId;
      return false;
    }

    public override int GetHashCode() { return base.GetHashCode(); }
  }
}