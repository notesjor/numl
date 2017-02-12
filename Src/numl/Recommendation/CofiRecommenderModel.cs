using System;
using System.Linq;
using numl.Math;
using numl.Math.LinearAlgebra;
using numl.Math.Metrics;
using numl.Model;
using numl.Utils;

namespace numl.Recommendation
{
  /// <summary>
  ///   Collaborative Filtering Recommender model.
  /// </summary>
  public class CofiRecommenderModel : Supervised.Model
  {
    /// <summary>
    ///   Initializes a new Collaborative Filtering recommender model.
    /// </summary>
    public CofiRecommenderModel()
    {
      RelatedDistanceFunction = new EuclidianDistance();
    }

    /// <summary>
    ///   Gets the Entity features mapping index of entity items and their corresponding row index.
    /// </summary>
    public Vector EntityFeatureMap { get; set; }

    /// <summary>
    ///   Gets or sets the average rating of the source items for each target.
    /// </summary>
    public Vector Mu { get; set; }

    /// <summary>
    ///   Gets the binary indicator matrix of references (i.e. ratings), where 1 indicates a value was provided otherwise 0.
    /// </summary>
    public Matrix R { get { return Reference.ToBinary(f => Ratings.Test(f)); } }

    /// <summary>
    ///   Gets or sets the Range of the ratings, values outside of this will be treated as not provided.
    /// </summary>
    public Range Ratings { get; set; }

    /// <summary>
    ///   Gets or sets the rating matrix R.
    ///   <para>Matrix of provided ratings, i.e. Book (m) ratings provided by Users (n) [m x n].</para>
    /// </summary>
    public Matrix Reference { get; set; }

    /// <summary>
    ///   Gets the Reference features mapping index of reference items and their corresponding col index.
    /// </summary>
    public Vector ReferenceFeatureMap { get; set; }

    /// <summary>
    ///   Gets or sets the function to use for minimizing the distance of related items.
    /// </summary>
    public IDistance RelatedDistanceFunction { get; set; }

    /// <summary>
    ///   Get or sets the Theta matrix containing the weights of each target item / feature pair.
    ///   <para>The target item is the learning objective, such as a Movie or Product.</para>
    /// </summary>
    public Matrix ThetaX { get; set; }

    /// <summary>
    ///   Get or sets the Theta matrix containing the weights for each source item / feature pair.
    ///   <para>The source item is the learning source, such as a User Review / Rating.</para>
    /// </summary>
    public Matrix ThetaY { get; set; }

    /// <summary>
    ///   Gets or sets the entity identifier labels.
    /// </summary>
    public Vector Y { get; set; }

    /// <summary>
    ///   Inserts a new learning target entity feature item into the model, i.e. insert a new Movie into the existing model.
    /// </summary>
    /// <param name="item">A source entity object.</param>
    public void NewEntity(object item)
    {
      if (Descriptor == null)
        throw new DescriptorException("A descriptor is required for inserting new entities into the model.");

      throw new NotImplementedException();
    }

    /// <summary>
    ///   Inserts a new source / reference feature (i.e. User rating items) into the existing model.
    /// </summary>
    /// <param name="item">A source feature object.</param>
    public void NewReference(object item)
    {
      if (Descriptor == null)
        throw new DescriptorException("A descriptor is required for inserting new references into the model.");

      throw new NotImplementedException();
    }

    /// <summary>
    ///   Not implemnted.
    /// </summary>
    /// <param name="y"></param>
    /// <returns></returns>
    public override double Predict(Vector y)
    {
      // check the input vector and see if it exists in the ratings matrix
      var featureIndex = Reference.GetCols().Where(w => w == y).Select((s, i) => i).FirstOrDefault();

      if (featureIndex >= 0)
        return Predict((int) ReferenceFeatureMap[featureIndex]).First();
      throw new Exception("Feature ratings for this item were not found in the reference collection");
    }

    /// <summary>
    ///   Predicts all the recommendations of the Items for the supplied reference, i.e. a user.
    /// </summary>
    /// <param name="referenceId">Reference index to use for generating predictions.</param>
    /// <returns>Vector of predictions.</returns>
    public Vector Predict(int referenceId)
    {
      // [entities x features] * [references * features]
      var predictions = (ThetaX * ThetaY.T).Each((v, r, c) => v + Mu[r]);

      int[] indices;
      predictions[ReferenceFeatureMap.IndexOf(referenceId), VectorType.Col].Sort(false, out indices);

      return Y.Slice(indices, true);
    }

    /// <summary>
    ///   Predicts the related items, given the item index and the type (either References or Entities).
    /// </summary>
    /// <param name="itemId">The item index in the corresponding feature map column.</param>
    /// <param name="count">count</param>
    /// <param name="itemType">
    ///   Type of item to return related items (i.e. References = user ratings OR Entities = books or
    ///   movies)
    /// </param>
    /// <returns>Vector of predictions.</returns>
    public Vector PredictRelated(int itemId, int count = 5, ItemType itemType = ItemType.References)
    {
      var predictions = (ThetaX * ThetaY.T).Each((v, r, c) => v + Mu[r]);

      var feature = itemType == ItemType.Entities
                      ? predictions[EntityFeatureMap.IndexOf(itemId), VectorType.Col]
                      : predictions[ReferenceFeatureMap.IndexOf(itemId), VectorType.Row];

      var result = Vector.Zeros(count);

      switch (itemType)
      {
        case ItemType.Entities:
        {
          result = predictions.GetCols()
                              .Select((s, i) => new {Col = s, Idx = i})
                              .OrderBy(v => RelatedDistanceFunction.Compute(feature, v.Col))
                              .Take(count)
                              .Select(s => (double) s.Idx).ToVector();
        }
          break;
        case ItemType.References:
        {
          result = predictions.GetRows()
                              .Select((s, i) => new {Row = s, Idx = i})
                              .OrderBy(v => RelatedDistanceFunction.Compute(feature, v.Row))
                              .Take(count)
                              .Select(s => (double) s.Idx).ToVector();
        }
          break;
      }

      return result;
    }
  }
}