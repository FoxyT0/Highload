namespace SpaceBattle.Lib;

using Hwdtech;
using System.Threading;

public class HardStopThreadCommand : SpaceBattle.Lib.ICommand
{
    string id;
    Action act;

    public HardStopThreadCommand(string id, Action act)
    {
        this.id = id;
        this.act = act;
    }
    public void Execute()
    {
        var mt = IoC.Resolve<MyThread>("Game.Threads.GetThread", id);
        var sd = IoC.Resolve<ISender>("Game.Threads.GetSender", id);
        sd.Send(
            IoC.Resolve<SpaceBattle.Lib.ICommand>("Game.Adapters.CommandAdapter", () =>
              {
                  if (Thread.CurrentThread == mt.thread)
                  {
                      act();
                      mt.Stop();
                  }
                  else
                  {
                      throw IoC.Resolve<Exception>("Game.Exceptions.WrongThread");
                  }
              }));
    }
}
