using numl.Data;

namespace numl.Supervised.NaiveBayes
{
  public class Label : IEdge
  {
    public string Text { get; set; }
    public int ChildId { get; set; }

    public int ParentId { get; set; }

    public static Label Create(IVertex parent, IVertex child, string text)
    {
      return new Label
      {
        Text = text,
        ParentId = parent.Id,
        ChildId = child.Id
      };
    }
  }
}