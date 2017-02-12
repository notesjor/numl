using numl.Data;

namespace numl.Tests.DataTests
{
  public class Vertex : IVertex
  {
    private static int _id;
    public Vertex() { Id = ++_id; }
    public int Id { get; set; }

    public string Label { get; set; }

    public override bool Equals(object obj)
    {
      if (obj is Vertex)
        return ((Vertex) obj).Id == Id && ((Vertex) obj).Label == Label;
      return false;
    }

    public override int GetHashCode() { return base.GetHashCode(); }

    public static void Reset() => _id = 0;
  }
}