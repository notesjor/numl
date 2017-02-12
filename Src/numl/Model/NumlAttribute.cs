using System;
using System.Reflection;
using numl.Utils;

namespace numl.Model
{
  /// <summary>Attribute for numl.</summary>
  [AttributeUsage(AttributeTargets.Property)]
  public abstract class NumlAttribute : Attribute
  {
    /// <summary>Generates a property.</summary>
    /// <param name="property">The property.</param>
    /// <returns>The property.</returns>
    public virtual Property GenerateProperty(PropertyInfo property)
    {
      return property.PropertyType.GenerateFeature(property.Name);
    }
  }
}