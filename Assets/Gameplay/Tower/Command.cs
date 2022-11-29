/// <summary>
/// Command Pattern
/// </summary>
public abstract class Command
{
    public abstract bool IsFinished { get; }
    public abstract void Execute();
}
