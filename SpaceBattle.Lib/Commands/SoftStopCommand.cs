namespace SpaceBattle.Lib;

using System.Threading;
using Hwdtech;

public class SoftStopCommand : ICommand
{
    MyThread thread;
    ISender sender;
    public SoftStopCommand(MyThread thread)
    {
        this.thread = thread;
        sender = IoC.Resolve<ISender>("Game.Queue.Sender", thread);
    }
    public void Execute()
    {
        if (Thread.CurrentThread == thread.thread)
        {
            var hscmd = new HardStopCommand(thread);
            sender.Send(hscmd);
        }
        else
        {
            throw IoC.Resolve<Exception>("Game.Exceptions.StoppingWrongThread");
        }
    }
}
