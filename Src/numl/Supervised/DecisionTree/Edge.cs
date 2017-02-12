using numl.Data;

namespace numl.Supervised.DecisionTree
{
  public class Edge : IEdge
  {
    /// <summary>Gets or sets the child identifier.</summary>
    /// <value>The child identifier.</value>
    public int ChildId { get; set; }
    /// <summary> Gets or sets the parent identifier.</summary>
    /// <value>The parent identifier.</value>
    public int ParentId { get; set; }
    /// <summary>Gets or sets the minimum.</summary>
    /// <value>The minimum value.</value>
    public double Min { get; set; }
    /// <summary>Gets or sets the maximum.</summary>
    /// <value>The maximum value.</value>
    public double Max { get; set; }
    /// <summary>Gets or sets a value indicating whether the discrete.</summary>
    /// <value>true if discrete, false if not.</value>
    public bool Discrete { get; set; }
    /// <summary>Gets or sets the label.</summary>
    /// <value>The label.</value>
    public string Label { get; set; }

    public override int GetHashCode()
    {
      return base.GetHashCode();
    }
    /// <summary>
    /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
    /// </summary>
    /// <param name="obj">The object to compare with the current object.</param>
    /// <returns><c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.</returns>
    public override bool Equals(object obj)
    {
      return obj is Edge &&
             ((Edge)obj).ChildId == ChildId &&
             ((Edge)obj).ParentId == ParentId &&
             ((Edge)obj).Min == Min &&
             ((Edge)obj).Max == Max &&
             ((Edge)obj).Discrete == Discrete &&
             ((Edge)obj).Label == Label;
    }
  }
}