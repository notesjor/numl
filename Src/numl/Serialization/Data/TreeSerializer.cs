using System;
using numl.Data;

namespace numl.Serialization.Data
{
  public class TreeSerializer : GraphSerializer
  {
    public override bool CanConvert(Type type) { return typeof(Tree).IsAssignableFrom(type); }

    public override object Create() { return new Tree(); }

    public override object Read(JsonReader reader)
    {
      if (reader.IsNull())
        return null;
      var t = base.Read(reader) as Tree;
      t.Root = reader.ReadProperty().Value as IVertex;
      return t;
    }

    public override void Write(JsonWriter writer, object value)
    {
      if (value == null)
      {
        writer.WriteNull();
      }
      else
      {
        var t = value as Tree;
        base.Write(writer, t);
        writer.WriteProperty(nameof(t.Root), t.Root);
      }
    }
  }
}