﻿namespace numl.AI.Functions
{
  /// <summary>
  ///   IHeuristicFunction interface.
  /// </summary>
  public interface IHeuristicFunction
  {
    /// <summary>
    ///   Computes the heuristic value for the given state.
    /// </summary>
    /// <param name="state">Current state.</param>
    /// <returns>Double.</returns>
    double Compute(IState state);
  }
}