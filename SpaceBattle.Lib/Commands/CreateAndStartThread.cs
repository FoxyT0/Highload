namespace SpaceBattle.Lib;

using Hwdtech;
using System.Collections.Concurrent;

public class CreateAndStartThreadCommand : SpaceBattle.Lib.ICommand
{
    string id;

    public CreateAndStartThreadCommand(string id)
    {
        this.id = id;
    }

    public void Execute()
    {
        BlockingCollection<SpaceBattle.Lib.ICommand> iq = new BlockingCollection<SpaceBattle.Lib.ICommand>();
		BlockingCollection<SpaceBattle.Lib.ICommand> oq = new BlockingCollection<SpaceBattle.Lib.ICommand>();

        IoC.Resolve<SpaceBattle.Lib.ICommand>("Game.Threads.SetInnerReciever", id, iq).Execute();
        IoC.Resolve<SpaceBattle.Lib.ICommand>("Game.Threads.SetInnerSender", id, iq).Execute();
        IoC.Resolve<SpaceBattle.Lib.ICommand>("Game.Threads.SetOuterReciever", id, oq).Execute();
        IoC.Resolve<SpaceBattle.Lib.ICommand>("Game.Threads.SetOuterSender", id, oq).Execute();

        MyThread mt = new MyThread(IoC.Resolve<IReciever>("Game.Threads.GetInnerReciever", id), 
		IoC.Resolve<IReciever>("Game.Threads.GetOuterReciever", id));
        
        IoC.Resolve<SpaceBattle.Lib.ICommand>("Game.Threads.SetThread", id, mt).Execute();
        mt.Execute();
    }
}

