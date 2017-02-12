// file:	Model\NumlAttributes.cs
//
// summary:	Implements the numl attributes class
using System;
using System.Reflection;
using System.Collections.Generic;
using numl.Utils;
using System.IO;

namespace numl.Model
{
    /// <summary>Attribute for numl.</summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public abstract class NumlAttribute : Attribute
    {
        /// <summary>Generates a property.</summary>
        /// <param name="property">The property.</param>
        /// <returns>The property.</returns>
        public virtual Property GenerateProperty(PropertyInfo property)
        {
            return TypeHelpers.GenerateFeature(property.PropertyType, property.Name);
        }
    }
}
