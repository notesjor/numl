using System;

namespace numl.Model
{
  /// <summary>Attribute for guid label.</summary>
  [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
  public class GuidLabelAttribute : LabelAttribute { }
}