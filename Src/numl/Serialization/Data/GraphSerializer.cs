using System.Linq;
using numl.Data;

namespace numl.Serialization.Data
{
  public class GraphSerializer : JsonSerializer<Graph>
  {
    public override object Read(JsonReader reader)
    {
      if (reader.IsNull())
        return null;
      var g = Create() as Graph;
      var vertices = reader.ReadArrayProperty()
                           .Value
                           .Select(o => (IVertex) o);
      foreach (var v in vertices)
        g.AddVertex(v);

      var edges = reader.ReadArrayProperty()
                        .Value
                        .Select(o => (IEdge) o);
      foreach (var e in edges)
        g.AddEdge(e);

      return g;
    }

    public override void Write(JsonWriter writer, object value)
    {
      if (value == null)
      {
        writer.WriteNull();
      }
      else
      {
        var g = (Graph) value;
        writer.WriteArrayProperty("Vertices", g.GetVertices());
        writer.WriteArrayProperty("Edges", g.GetEdges());
      }
    }
  }
}