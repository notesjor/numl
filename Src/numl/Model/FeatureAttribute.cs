using System;

namespace numl.Model
{
  /// <summary>Attribute for feature.</summary>
  [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
  public class FeatureAttribute : NumlAttribute { }
}