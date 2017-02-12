﻿using System;
using System.Collections.Generic;
using System.Text;
using numl.AI;
using numl.Data;

namespace numl.Tests.AITests
{
  public class TicTacToe : IAdversarialState
  {
    private static int _Id;

    private readonly int[] _board = {0, 0, 0, 0, 0, 0, 0, 0, 0};

    public TicTacToe(bool player)
    {
      Id = ++_Id;
      IsTerminal = false;
      Player = player;
    }

    public TicTacToe(bool player, int[] board)
      : this(player)
    {
      Id = ++_Id;
      _board = board;
      Calculate();
    }

    public int Id { get; set; }
    public bool IsTerminal { get; private set; }
    public bool Player { get; }
    public double Utility { get; private set; }

    private void Calculate()
    {
      Utility = 0;
      IsTerminal = false;

      var wins = new[]
      {
        new[] {0, 1, 2},
        new[] {3, 4, 5},
        new[] {6, 7, 8},
        new[] {0, 3, 6},
        new[] {1, 4, 7},
        new[] {2, 5, 8},
        new[] {0, 4, 8},
        new[] {2, 4, 6}
      };

      foreach (var w in wins)
      {
        var u = Calculate(w);
        if (u != 0)
        {
          Utility = u;
          IsTerminal = u != 0;
          return;
        }
      }
    }

    private int Calculate(int[] w)
    {
      if (w.Length != 3)
        throw new InvalidOperationException("Needs to be three!");

      if (Math.Abs(_board[w[0]] + _board[w[1]] + _board[w[2]]) == 3)
        return _board[w[0]] < 0 ? -1 : 1;
      return 0;
    }

    public int CompareTo(object obj) { return StateComparer.Compare(this, obj as IAdversarialState); }

    public override bool Equals(object obj) { return IsEqualTo(obj as IState); }

    public override int GetHashCode() { return _board.GetHashCode(); }

    public IEnumerable<ISuccessor> GetSuccessors()
    {
      for (var i = 0; i < _board.Length; i++)
        if (_board[i] == 0)
        {
          var play = Player ? 1 : -1;
          var newBoard = (int[]) _board.Clone();
          newBoard[i] = play;
          yield return new TicTacToeMove(new TicTacToe(!Player, newBoard), new Action(i.ToString()));
        }
    }

    public double Heuristic()
    {
      // not applicable in this case
      return 1;
    }

    public bool IsEqualTo(IVertex state)
    {
      if (state == null)
        return false;
      if (!(state is TicTacToe))
        return false;

      var tictactoe = (TicTacToe) state;
      for (var i = 0; i < _board.Length; i++)
        if (_board[i] != tictactoe._board[i])
          return false;

      return true;
    }

    public IAdversarialState Reset() { return new TicTacToe(Player); }

    public override string ToString()
    {
      var sb = new StringBuilder();
      for (var i = 0; i < _board.Length; i++)
      {
        var p = '_';
        if (_board[i] < 0)
          p = 'o';
        if (_board[i] > 0)
          p = 'x';

        sb.Append(string.Format(" {0} ", p));
        if ((i + 1) % 3 == 0)
          sb.Append(" \n");
      }
      return sb.ToString();
    }
  }
}