using System;
using System.Reflection;

namespace numl.Model
{
  /// <summary>Attribute for string feature.</summary>
  [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
  public class StringFeatureAttribute : FeatureAttribute
  {
    /// <summary>Gets or sets the type of the split.</summary>
    /// <value>The type of the split.</value>
    public StringSplitType SplitType { get; set; }
    /// <summary>Gets or sets the separator.</summary>
    /// <value>The separator.</value>
    public string Separator { get; set; }
    /// <summary>Gets or sets the exclusion file as a base64 encoded string.</summary>
    /// <value>The exclusion file.</value>
    public string ExclusionFile { get; set; }
    /// <summary>Gets or sets a value indicating whether as enum.</summary>
    /// <value>true if as enum, false if not.</value>
    public bool AsEnum { get; set; }

    /// <summary>Default constructor.</summary>
    public StringFeatureAttribute()
    {
      AsEnum = false;
      SplitType = StringSplitType.Word;
      Separator = " ";
    }
    /// <summary>Constructor.</summary>
    /// <param name="splitType">Type of the split.</param>
    /// <param name="separator">(Optional) the separator.</param>
    /// <param name="exclusions">(Optional) the exclusions.</param>
    public StringFeatureAttribute(StringSplitType splitType, string separator = " ", string exclusions = null)
    {
      SplitType = splitType;
      Separator = separator;
      ExclusionFile = exclusions;
    }
    /// <summary>Constructor.</summary>
    /// <param name="asEnum">true to as enum.</param>
    public StringFeatureAttribute(bool asEnum)
    {
      AsEnum = asEnum;
    }
    /// <summary>Generates a property.</summary>
    /// <exception cref="InvalidOperationException">Thrown when the requested operation is invalid.</exception>
    /// <param name="property">The property.</param>
    /// <returns>The property.</returns>
    public override Property GenerateProperty(PropertyInfo property)
    {
      if (property.PropertyType != typeof(string))
        throw new InvalidOperationException("Must use a string property.");

      var sp = new StringProperty
      {
        Name = property.Name,
        SplitType = SplitType,
        Separator = Separator,
        AsEnum = AsEnum,
        Discrete = true
      };

      if (!string.IsNullOrWhiteSpace(ExclusionFile))
        sp.ImportExclusions(ExclusionFile);


      return sp;
    }
  }
}