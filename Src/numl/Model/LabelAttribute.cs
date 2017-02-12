using System;
using System.Reflection;
using numl.Utils;

namespace numl.Model
{
  /// <summary>Attribute for label.</summary>
  [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
  public class LabelAttribute : NumlAttribute
  {
    /// <summary>Generates a property.</summary>
    /// <param name="property">The property.</param>
    /// <returns>The property.</returns>
    public override Property GenerateProperty(PropertyInfo property)
    {
      return TypeHelpers.GenerateLabel(property.PropertyType, property.Name);
    }
  }
}