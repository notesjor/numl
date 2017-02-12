using System;
using System.Collections.Generic;
using System.Linq;
using numl.AI;
using numl.Math;
using numl.Math.Discretization;
using numl.Math.LinearAlgebra;
using Action = numl.AI.Action;

namespace numl.Reinforcement.States
{
  /// <summary>
  ///   Converter class for generating Markov Decision Processes.
  /// </summary>
  public static class MDPConverter
  {
    /// <summary>
    ///   Converts the experience pair into their equivalent math forms.
    /// </summary>
    /// <param name="state">IMDPState instance.</param>
    /// <param name="nodes">List of nodes added to the result set.</param>
    /// <param name="states">Matrix to store contained successor state vectors.</param>
    /// <param name="actions">Vector to store the contained action values.</param>
    /// <param name="statesP">Matrix to store all contained successor transition state vectors.</param>
    /// <param name="rewards">Vector to store all contained reward values.</param>
    /// <returns>HashSet&lt;string&gt;</returns>
    private static void Convert(
      this IMDPState state,
      ref List<string> nodes,
      ref Matrix states,
      ref Vector actions,
      ref Matrix statesP,
      ref Vector rewards)
    {
      if (state != null)
        foreach (IMDPSuccessor successor in state.GetSuccessors())
        {
          if (state.Features.Length != states.Cols)
            states = Matrix.Reshape(states, states.Rows, state.Features.Length);
          if (state.Features.Length != statesP.Cols)
            statesP = Matrix.Reshape(statesP, statesP.Rows, ((IMDPState) successor.State).Features.Length);

          string id = $"{state.Id}:{successor.State.Id}";
          if (!nodes.Contains(id))
          {
            states = states.Insert(state.ToVector(), states.Rows - 1, VectorType.Row);
            actions = actions.Insert(actions.Length - 1, successor.Action.Id);
            statesP = statesP.Insert(((IMDPState) successor.State).ToVector(), statesP.Rows - 1, VectorType.Row);
            rewards = rewards.Insert(rewards.Length - 1, successor.Reward);
            nodes.Add(id);
          }

          if (!successor.State.IsTerminal)
          {
            var successorState = (IMDPState) successor.State;
            if (successorState.Id != state.Id)
              successorState.Convert(ref nodes, ref states, ref actions, ref statesP, ref rewards);
          }
        }
    }

    /// <summary>
    ///   Converts a double into an <see cref="IAction" />.
    /// </summary>
    /// <param name="action">Double.</param>
    /// <param name="parentStateId">Parent state identifier.</param>
    /// <param name="childStateId">Child state identifier.</param>
    /// <returns>IAction</returns>
    public static Action GetAction(double action, int parentStateId, int childStateId)
    {
      return new Action
      {
        Name = Action.GetActionId(action).ToString(),
        Id = (int) action,
        Probability = 1.0,
        ParentId = parentStateId,
        ChildId = childStateId
      };
    }

    private static string GetActionKey(int stateId, int tStateId) { return $"{stateId}:{tStateId}"; }

    /// <summary>
    ///   Converts the state vector into an MDP state.
    /// </summary>
    /// <param name="state">State vector.</param>
    /// <param name="summary">Feature properties from the original set.</param>
    /// <param name="discretizer">Discretization function for generating unique state identifiers.</param>
    /// <returns>MDPState.</returns>
    public static MDPState GetState(Vector state, Summary summary, IDiscretizer discretizer)
    {
      return new MDPState((int) discretizer.Discretize(state, summary)) {Features = state};
    }

    /// <summary>
    ///   Returns a graph of MDP States from the States matrices and Action label vector.
    /// </summary>
    /// <param name="states">State matrix.</param>
    /// <param name="actions">Action label vector.</param>
    /// <param name="statesP">Transition states matrix.</param>
    /// <param name="reward">Reward value vector.</param>
    /// <param name="properties">Feature properties from the original set.</param>
    /// <param name="discretizer">Discretization function for generating unique state identifiers.</param>
    /// <returns>IEnumerable&lt;IMDPState&gt;</returns>
    public static IEnumerable<MDPState> GetStates(
      Matrix states,
      Vector actions,
      Matrix statesP,
      Vector reward,
      Summary properties,
      IDiscretizer discretizer)
    {
      var summary = properties ?? Summary.Summarize(states);

      discretizer.Initialize(states, summary);

      var sdist = new Dictionary<double, MDPState>();
      var adist = new Dictionary<string, double>();
      var results = new Dictionary<double, MDPState>();

      for (var i = 0; i < states.Rows; i++)
      {
        var sid = discretizer.Discretize(states[i], summary);

        if (!sdist.ContainsKey(sid))
        {
          sdist.Add(sid, GetState(states[i], summary, discretizer));
          results.Add(sid, sdist[sid]);
        }

        var tsid = discretizer.Discretize(statesP[i], summary);
        var tstate = sdist.ContainsKey(tsid) ? sdist[tsid] : GetState(statesP[i], summary, discretizer);

        if (!sdist.ContainsKey(tsid))
          sdist.Add(tsid, tstate);

        var key = GetActionKey((int) sid, (int) tsid);

        if (!adist.ContainsKey(key))
          adist.Add(key, 1);
        else
          adist[key]++;

        sdist[sid].Successors.Add(
          new MDPSuccessorState(GetAction(actions[i], (int) sid, (int) tsid), 0, tstate, reward[i]));

        if (results.ContainsKey(tsid))
          results.Remove(tsid);
      }

      foreach (var state in sdist.Values)
      {
        var sum = state.Successors.Sum(s => adist[GetActionKey(state.Id, s.State.Id)]);
        foreach (var successor in state.Successors)
        {
          var key = GetActionKey(state.Id, successor.State.Id);
          ((Action) successor.Action).Probability = adist[key] / sum;
        }
      }

      // return starting states
      return results.Values;
    }

    /// <summary>
    ///   Returns a flat collection of states/actions and their transition states.
    /// </summary>
    /// <param name="states">State matrix.</param>
    /// <param name="actions">Action label vector.</param>
    /// <param name="statesP">Transition states matrix.</param>
    /// <param name="properties">(Optional) Feature summary.</param>
    /// <param name="discretizer">Disretization function to apply for reducing states.</param>
    /// <returns></returns>
    public static Tuple<IEnumerable<IState>, IEnumerable<IAction>, IEnumerable<IState>> GetStates(
      Matrix states,
      Vector actions,
      Matrix statesP,
      Summary properties,
      IDiscretizer discretizer)
    {
      var summary = properties ?? Summary.Summarize(states);

      var slist = new IState[states.Rows];
      var alist = new IAction[actions.Length];
      var splist = new IState[statesP.Rows];

      for (var i = 0; i < states.Rows; i++)
      {
        slist[i] = GetState(states[i], summary, discretizer);
        splist[i] = GetState(statesP[i], summary, discretizer);
        alist[i] = GetAction(actions[i], slist[i].Id, splist[i].Id);
      }

      return new Tuple<IEnumerable<IState>, IEnumerable<IAction>, IEnumerable<IState>>(slist, alist, splist);
    }

    /// <summary>
    ///   Converts a Matrix of states into an array of State objects.
    /// </summary>
    /// <param name="states">State matrix.</param>
    /// <param name="properties">(Optional) Feature summary.</param>
    /// <param name="discretizer">Disretization function to apply for reducing states.</param>
    /// <returns></returns>
    public static IEnumerable<IMDPState> GetStates(Matrix states, Summary properties, IDiscretizer discretizer)
    {
      var summary = properties ?? Summary.Summarize(states);

      var slist = new IMDPState[states.Rows];

      for (var i = 0; i < states.Rows; i++)
        slist[i] = GetState(states[i], summary, discretizer);

      return slist;
    }

    /// <summary>
    ///   Converts an MDP State (recursively) into it's equivalent math form, including all successor states.
    /// </summary>
    /// <param name="state">The starting state.</param>
    /// <returns>Tuple&lt;Matrix, Vector, Matrix&gt;</returns>
    public static Tuple<Matrix, Vector, Matrix, Vector> ToExamples(this IMDPState state)
    {
      return new[] {state}.ToExamples();
    }

    /// <summary>
    ///   Converts an enumerable of MDP States (recursively) into their equivalent math forms, including their successor
    ///   states.
    /// </summary>
    /// <param name="states">An enumerable of states.</param>
    /// <returns>Tuple&lt;Matrix, Vector, Matrix&gt;</returns>
    public static Tuple<Matrix, Vector, Matrix, Vector> ToExamples(this IEnumerable<IMDPState> states)
    {
      int width = states.First().Features.Length, height = states.Count();

      var statesM = new Matrix(0);
      var actions = new Vector(0);
      var statesMP = new Matrix(0);
      var rewards = new Vector(0);
      var ids = new List<string>();

      foreach (var state in states)
        state.Convert(ref ids, ref statesM, ref actions, ref statesMP, ref rewards);

      return new Tuple<Matrix, Vector, Matrix, Vector>(statesM, actions, statesMP, rewards);
    }

    /// <summary>
    ///   Converts the MDP State into a vector form.
    /// </summary>
    /// <param name="state">MDP State.</param>
    /// <returns>Vector.</returns>
    public static Vector ToVector(this IMDPState state)
    {
      return state.Features;
    }
  }
}