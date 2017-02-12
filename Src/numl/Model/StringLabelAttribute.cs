using System;

namespace numl.Model
{
  /// <summary>Attribute for string label.</summary>
  [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
  public class StringLabelAttribute : LabelAttribute { }
}