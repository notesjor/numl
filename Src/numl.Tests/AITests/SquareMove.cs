using numl.AI;

namespace numl.Tests.AITests
{
  public class SquareMove : ISuccessor
  {
    public SquareMove(IState state, IAction action)
    {
      State = state;
      Action = action;
    }

    public IAction Action { get; private set; }

    public double Cost { get { return 1; } }
    public IState State { get; private set; }
  }
}