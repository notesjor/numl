namespace numl.Tests.AITests
{
  public class SquareMove : ISuccessor
  {

    public SquareMove(IState state, IAction action)
    {
      State = state;
      Action = action;
    }

    public double Cost { get { return 1; } }
    public IAction Action { get; private set; }
    public IState State { get; private set; }
  }
}