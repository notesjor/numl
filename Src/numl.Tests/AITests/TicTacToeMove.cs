namespace numl.Tests.AITests
{
  public class TicTacToeMove : ISuccessor
  {

    public TicTacToeMove(IState state, IAction action)
    {
      State = state;
      Action = action;
    }

    public double Cost
    {
      get { return 1; }
    }

    public IAction Action { get; private set; }

    public IState State { get; private set; }

  }
}