namespace SpaceBattle.Lib;

using Hwdtech;
using System.Collections.Concurrent;

public class SoftStopThreadCommand : SpaceBattle.Lib.ICommand
{
    string id;
    Action act;

    public SoftStopThreadCommand(string id, Action act)
    {
        this.id = id;
        this.act = act;
    }
    public void Execute()
    {
        var mt = IoC.Resolve<MyThread>("Game.Threads.GetThread", id);
        var fakeq = new BlockingCollection<SpaceBattle.Lib.ICommand>();
        var sd = IoC.Resolve<ISender>("Game.Threads.GetSender", id);
        var rc = IoC.Resolve<IReciever>("Game.Threads.GetReciever", id);
        Action a = () =>
        {
            if (rc.isEmpty())
            {
                act();
                mt.Stop();
            }
        };
        sd.Send(new UpdateBehaviorCommand(mt, a));
        IoC.Resolve<SpaceBattle.Lib.ICommand>("Game.Threads.SetSender", id, fakeq).Execute();
    }
}

