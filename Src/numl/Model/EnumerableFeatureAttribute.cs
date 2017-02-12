using System;
using System.Collections;
using System.Linq;
using System.Reflection;

namespace numl.Model
{
  /// <summary>Attribute for enumerable feature.</summary>
  [AttributeUsage(AttributeTargets.Property)]
  public class EnumerableFeatureAttribute : FeatureAttribute
  {
    /// <summary>The length.</summary>
    private readonly int _length;

    /// <summary>Constructor.</summary>
    /// <param name="length">The length.</param>
    public EnumerableFeatureAttribute(int length) { _length = length; }

    /// <summary>Generates a property.</summary>
    /// <exception cref="InvalidOperationException">Thrown when the requested operation is invalid.</exception>
    /// <param name="property">The property.</param>
    /// <returns>The property.</returns>
    public override Property GenerateProperty(PropertyInfo property)
    {
      if (!property.PropertyType.GetInterfaces().Contains(typeof(IEnumerable)))
        throw new InvalidOperationException("Invalid Enumerable type.");

      if (_length <= 0)
        throw new InvalidOperationException("Cannot have an enumerable feature of 0 or less.");

      var type = property.PropertyType;
      // good assumption??
      // TODO: Check assumptions on enums
      return new EnumerableProperty(_length)
      {
        Discrete = type == typeof(bool) ||
                   type == typeof(char),
        Name = property.Name,
        Type = type.GetElementType()
      };      
    }
  }
}