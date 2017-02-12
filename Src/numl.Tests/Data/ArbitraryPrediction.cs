﻿using System.Collections.Generic;
using numl.Model;

namespace numl.Tests.Data
{
  public class ArbitraryPrediction
  {
    public enum PredictionLabel
    {
      Minimum,
      Medium,
      Maximum
    }

    [Feature]
    public decimal FirstTestFeature { get; set; }

    [Label]
    public PredictionLabel OutcomeLabel { get; set; }

    [Feature]
    public decimal SecondTestFeature { get; set; }

    [Feature]
    public decimal ThirdTestFeature { get; set; }

    public static ArbitraryPrediction[] GetData()
    {
      var returnData = new List<ArbitraryPrediction>();

      for (var i = 0; i < 80; i++)
        returnData.Add(
          new ArbitraryPrediction
          {
            FirstTestFeature = 1.0m,
            SecondTestFeature = i,
            ThirdTestFeature = 1.2m,
            OutcomeLabel = i < 50 ? PredictionLabel.Minimum : PredictionLabel.Maximum
          });

      return returnData.ToArray();
    }

    public static IEnumerable<ArbitraryPrediction> GetDataUsingNamedIterator()
    {
      for (var i = 0; i < 80; i++)
        yield return new ArbitraryPrediction
        {
          FirstTestFeature = 1.0m,
          SecondTestFeature = i,
          ThirdTestFeature = 1.2m,
          OutcomeLabel = i < 50 ? PredictionLabel.Minimum : PredictionLabel.Maximum
        };
    }
  }
}