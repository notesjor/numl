using System;
using System.Reflection;

namespace numl.Model
{
  /// <summary>Attribute for guid feature.</summary>
  [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
  public class GuidFeatureAttribute : FeatureAttribute
  {
    /// <summary>Default constructor.</summary>
    public GuidFeatureAttribute()
    {
    }
    /// <summary>Generates a property.</summary>
    /// <exception cref="InvalidOperationException">Thrown when the requested operation is invalid.</exception>
    /// <param name="property">The property.</param>
    /// <returns>The property.</returns>
    public override Property GenerateProperty(PropertyInfo property)
    {
      if (property.PropertyType != typeof(Guid))
        throw new InvalidOperationException("Must use a guid property.");

      var gp = new GuidProperty
      {
        Name = property.Name,
        Discrete = true
      };
			
      return gp;
    }
  }
}