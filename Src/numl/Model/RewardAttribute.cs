using System;
using System.Reflection;
using numl.Utils;

namespace numl.Model
{
  /// <summary>
  /// Attribute for a Reward value.
  /// </summary>
  [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
  public class RewardAttribute : NumlAttribute
  {
    /// <summary>
    /// Default constructor.
    /// </summary>
    public RewardAttribute() { }

    /// <summary>Generates a property.</summary>
    /// <exception cref="InvalidOperationException">Thrown when the requested operation is invalid.</exception>
    /// <param name="property">The property.</param>
    /// <returns>The property.</returns>
    public override Property GenerateProperty(PropertyInfo property)
    {
      return TypeHelpers.GenerateLabel(property.PropertyType, property.Name);
    }
  }
}