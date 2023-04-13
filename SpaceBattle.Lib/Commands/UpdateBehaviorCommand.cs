namespace SpaceBattle.Lib;

public class UpdateBehaviorCommand : ICommand
{
    Action behavior;
    MyThread thread;

    public UpdateBehaviorCommand(MyThread thread, Action behavior)
    {
        this.thread = thread;
        this.behavior = behavior;
    }

    public void Execute()
    {
        thread.UpdateBehavior(behavior);
    }
}
