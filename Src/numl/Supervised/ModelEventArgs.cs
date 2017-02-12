using System;

namespace numl.Supervised
{
  /// <summary>Additional information for model events.</summary>
  public class ModelEventArgs : EventArgs
  {
    /// <summary>Constructor.</summary>
    /// <param name="model">The model.</param>
    /// <param name="message">(Optional) the message.</param>
    public ModelEventArgs(IModel model, string message = "")
    {
      Message = message;
      Model = model;
    }

    /// <summary>Gets or sets the message.</summary>
    /// <value>The message.</value>
    public string Message { get; private set; }

    /// <summary>Gets or sets the model.</summary>
    /// <value>The model.</value>
    public IModel Model { get; private set; }

    /// <summary>Makes.</summary>
    /// <param name="model">The model.</param>
    /// <param name="message">(Optional) the message.</param>
    /// <returns>The ModelEventArgs.</returns>
    internal static ModelEventArgs Make(IModel model, string message = "")
    {
      return new ModelEventArgs(model, message);
    }
  }
}