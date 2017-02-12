﻿using System;
using System.Linq;
using numl.Model;

namespace numl.Serialization.Model
{
  public class GuidPropertySerializer : PropertySerializer
  {
    public override bool CanConvert(Type type) { return typeof(GuidProperty).IsAssignableFrom(type); }

    public override object Create() { return new GuidProperty(); }

    public override object Read(JsonReader reader)
    {
      var p = (GuidProperty) base.Read(reader);
      p.Categories = reader.ReadArrayProperty().Value
                           .Select(o => Guid.Parse((string) o))
                           .ToArray();

      return p;
    }

    public override void Write(JsonWriter writer, object value)
    {
      base.Write(writer, value);
      var p = (GuidProperty) value;
      writer.WriteArrayProperty("Categories", p.Categories);
    }
  }
}