using numl.Supervised;

namespace numl
{
  /// <summary>Structure to hold generator, model, and accuracy information.</summary>
  public class LearningModel
  {
    /// <summary>Gets the overall Accuracy of the model.</summary>
    /// <value>The accuracy.</value>
    public double Accuracy { get { return Score.Accuracy; } }

    /// <summary>Generator used to create model.</summary>
    /// <value>The generator.</value>
    public IGenerator Generator { get; set; }

    /// <summary>Model created by generator.</summary>
    /// <value>The model.</value>
    public IModel Model { get; set; }

    /// <summary>
    ///   Gets the Score of the model.
    /// </summary>
    public Score Score { get; set; }

    /// <summary>Textual representation of structure.</summary>
    /// <returns>string.</returns>
    public override string ToString()
    {
      return
        $"Learning Model:\n  Generator {Generator}\n  Model:\n{Model}\n  Accuracy: {Accuracy:p}\n  Precision: {Score.Precision:p}\n  Recall: {Score.Recall:p}\n  F-Score: {Score.FScore:p}";
    }
  }
}