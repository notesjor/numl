using System;
using System.Collections;
using System.Linq;
using System.Reflection;

namespace numl.Model
{
  /// <summary>Attribute for enumerable feature.</summary>
  [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
  public class EnumerableFeatureAttribute : FeatureAttribute
  {
    /// <summary>The length.</summary>
    private readonly int _length;
    /// <summary>Constructor.</summary>
    /// <param name="length">The length.</param>
    public EnumerableFeatureAttribute(int length)
    {
      _length = length;
    }
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

      Type type = property.PropertyType;
      var ep = new EnumerableProperty(_length);
      // good assumption??
      // TODO: Check assumptions on enums

      ep.Discrete = //type.BaseType == typeof(Enum) ||
        type == typeof(bool) ||
        type == typeof(char);
      ep.Name = property.Name;

      ep.Type = type.GetElementType();
      return ep;
    }
  }
}