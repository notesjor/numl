namespace numl.Tests.DataTests
{
  public class Vertex : IVertex
  {
    static int _id = 0;
    public static void Reset() => _id = 0;
    public Vertex() { Id = ++_id; }
    public int Id { get; set; }

    public string Label { get; set; }

    public override int GetHashCode()
    {
      return base.GetHashCode();
    }
    public override bool Equals(object obj)
    {
      if (obj is Vertex)
        return ((Vertex)obj).Id == Id && ((Vertex)obj).Label == Label;
      else
        return false;
    }
  }
}