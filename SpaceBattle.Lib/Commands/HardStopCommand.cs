namespace SpaceBattle.Lib;

using System.Threading;
using Hwdtech;

public class HardStopCommand : ICommand
{
    MyThread thread;

    public HardStopCommand(MyThread thread)
    {
        this.thread = thread;
    }
    public void Execute()
    {
        if (Thread.CurrentThread == thread.thread)
        {
            thread.Stop();
        }
        else
        {
            throw new Exception();
        }
    }
}
