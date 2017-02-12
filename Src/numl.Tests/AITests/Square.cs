using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using numl.AI;
using numl.Data;

namespace numl.Tests.AITests
{
  public class Square : IState
  {
    private static int _Id;
    private readonly int[] _modeIdx = {-1, 1, -3, 3};

    private readonly Action[] _moves = new[] {"Left", "Right", "Up", "Down"}
      .Select(s => new Action(s)).ToArray();

    private readonly int[] _square = new int[9];

    public Square(int[] square)
    {
      _square = square;
      IsTerminal = CalculateTerminal(_square);
      Id = ++_Id;
    }

    public int Id { get; set; }
    public bool IsTerminal { get; private set; }

    private static bool CalculateTerminal(int[] square)
    {
      for (var i = 0; i < square.Length; i++)
        if (i != square[i])
          return false;
      return true;
    }

    public int CompareTo(object obj) { return StateComparer.Compare(this, obj as IState); }

    public override bool Equals(object obj) { return IsEqualTo(obj as IState); }

    public override int GetHashCode() { return _square.GetHashCode(); }

    public IEnumerable<ISuccessor> GetSuccessors()
    {
      for (var i = 0; i < 4; i++)
      {
        var move = _moves[i];
        var idx = Array.IndexOf(_square, 0);

        if (Test(idx, move))
        {
          var shift = _modeIdx[i];
          yield return new SquareMove(Swap(idx, idx + shift), move);
        }
      }
    }

    public double Heuristic()
    {
      // heuristic for missplaced items
      var problem = 0;
      for (var i = 0; i < _square.Length; i++)
        if (i != _square[i])
          problem++;
      return problem;
    }

    public bool IsEqualTo(IVertex state)
    {
      if (state == null)
        return false;
      if (!(state is Square))
        return false;
      var square = (Square) state;
      for (var i = 0; i < _square.Length; i++)
        if (_square[i] != square._square[i])
          return false;
      return true;
    }

    private IState Swap(int a, int b)
    {
      var newSquare = (int[]) _square.Clone();
      var t = newSquare[a];
      newSquare[a] = newSquare[b];
      newSquare[b] = t;
      return new Square(newSquare);
    }

    public static bool Test(int idx, IAction action)
    {
      if (action.Name == "Left" && idx % 3 != 0)
        return true;
      if (action.Name == "Right" && (idx + 1) % 3 != 0)
        return true;
      if (action.Name == "Up" && idx > 2)
        return true;
      if (action.Name == "Down" && idx < 6)
        return true;
      return false;
    }

    public override string ToString()
    {
      var sb = new StringBuilder();
      for (var i = 0; i < _square.Length; i++)
      {
        sb.Append(string.Format("  {0} ", _square[i] == 0 ? "_" : _square[i].ToString()));
        if ((i + 1) % 3 == 0)
          sb.Append("\n");
      }
      return sb.ToString();
    }
  }
}