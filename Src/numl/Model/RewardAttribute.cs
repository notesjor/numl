using System;
using System.Reflection;
using numl.Utils;

namespace numl.Model
{
  /// <summary>
  ///   Attribute for a Reward value.
  /// </summary>
  [AttributeUsage(AttributeTargets.Property)]
  public class RewardAttribute : NumlAttribute
  {
    /// <summary>Generates a property.</summary>
    /// <exception cref="InvalidOperationException">Thrown when the requested operation is invalid.</exception>
    /// <param name="property">The property.</param>
    /// <returns>The property.</returns>
    public override Property GenerateProperty(PropertyInfo property)
    {
      return property.PropertyType.GenerateLabel(property.Name);
    }
  }
}