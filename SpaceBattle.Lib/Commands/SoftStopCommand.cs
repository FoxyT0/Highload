namespace SpaceBattle.Lib;

using System.Threading;
using Hwdtech;

class SoftStopCommand : ICommand
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
            sender.Send(IoC.Resolve<ICommand>("Game.Commands.HardStopCommand"));
        }
        else
        {
            throw IoC.Resolve<Exception>("Game.Exceptions.StoppingWrongThread");
        }
    }
}
