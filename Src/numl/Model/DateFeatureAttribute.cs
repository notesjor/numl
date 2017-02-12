using System;
using System.Reflection;

namespace numl.Model
{
  /// <summary>Attribute for date feature.</summary>
  [AttributeUsage(AttributeTargets.Property)]
  public class DateFeatureAttribute : FeatureAttribute
  {
    /// <summary>The dp.</summary>
    private readonly DateTimeProperty dp;

    /// <summary>Constructor.</summary>
    /// <param name="features">The features.</param>
    public DateFeatureAttribute(DateTimeFeature features) { dp = new DateTimeProperty(features); }

    /// <summary>Constructor.</summary>
    /// <param name="portion">The portion.</param>
    public DateFeatureAttribute(DatePortion portion) { dp = new DateTimeProperty(portion); }

    /// <summary>Generates a property.</summary>
    /// <exception cref="InvalidOperationException">Thrown when the requested operation is invalid.</exception>
    /// <param name="property">The property.</param>
    /// <returns>The property.</returns>
    public override Property GenerateProperty(PropertyInfo property)
    {
      if (property.PropertyType != typeof(DateTime))
        throw new InvalidOperationException("Invalid datetime property.");

      dp.Discrete = true;
      dp.Name = property.Name;
      return dp;
    }
  }
}