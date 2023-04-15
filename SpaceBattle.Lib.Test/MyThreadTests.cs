namespace SpaceBattle.Lib.Test;

using System.Collections.Concurrent;
using Hwdtech;
using Hwdtech.Ioc;
using Moq;

public class MyThreadUnitTests
{
    object ic;
    public MyThreadUnitTests()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();
        ic = IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"));
        IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", ic).Execute();
    }

    [Fact]
    public void RecieverAdapterTake()
    {
        var cmd = new Mock<SpaceBattle.Lib.ICommand>();
        cmd.Setup(obj => obj.Execute()).Verifiable();
        var q = new BlockingCollection<SpaceBattle.Lib.ICommand>() { cmd.Object };
        var ra = new RecieverAdapter(q);

        ra.Recieve().Execute();

        cmd.Verify(obj => obj.Execute());
    }

    [Fact]
    public void RecieverAdapterIsBool()
    {
        var q = new BlockingCollection<SpaceBattle.Lib.ICommand>() { };
        var ra = new RecieverAdapter(q);

        bool a = ra.isEmpty();

        Assert.True(a);
    }

    [Fact]
    public void PositiveMakeThreadsTest()
    {
        var waiter = new AutoResetEvent(false);
        var cmd = new Mock<SpaceBattle.Lib.ICommand>();
        cmd.Setup(obj => obj.Execute()).Callback(() => waiter.Set()).Verifiable();
        var rc = new Mock<IReciever>();
        rc.Setup(obj => obj.Recieve()).Returns(cmd.Object);
        var mt = new MyThread(rc.Object);

        mt.Execute();

        waiter.WaitOne();
        cmd.Verify(obj => obj.Execute());
    }

    [Fact]
    public void UpdateBehaviorTest()
    {
        var waiter = new AutoResetEvent(false);
        var cmd = new Mock<SpaceBattle.Lib.ICommand>();
        cmd.Setup(obj => obj.Execute());
        var rc = new Mock<IReciever>();
        rc.Setup(obj => obj.Recieve()).Returns(cmd.Object);
        var fakecmd = new Mock<SpaceBattle.Lib.ICommand>();
        fakecmd.Setup(obj => obj.Execute()).Callback(() => waiter.Set()).Verifiable();
        var newbhvr = new Action(() => fakecmd.Object.Execute());
        var mt = new MyThread(rc.Object);
        var ubcmd = new UpdateBehaviorCommand(mt, newbhvr);

        ubcmd.Execute();
        mt.Execute();

        waiter.WaitOne();
        fakecmd.Verify(obj => obj.Execute());
    }

    [Fact]
    public void HardStopCommandTest()
    {
        var rc = new Mock<IReciever>();
        var mt = new MyThread(rc.Object);
        var hscmd = new HardStopCommand(mt);
        var cmd = new Mock<SpaceBattle.Lib.ICommand>();
        cmd.Setup(obj => obj.Execute()).Verifiable();
        var q = new BlockingCollection<SpaceBattle.Lib.ICommand>();
        q.Add(hscmd);
        q.Add(cmd.Object);

        rc.Setup(obj => obj.Recieve()).Returns(q.Take);

        mt.Execute();

        cmd.Verify(obj => obj.Execute(), Times.Never());
    }

    class FakeSender : IStrategy
    {
        BlockingCollection<SpaceBattle.Lib.ICommand> q;

        public FakeSender(BlockingCollection<SpaceBattle.Lib.ICommand> q)
        {
            this.q = q;
        }

        public object run_strategy(params object[] args)
        {
            var hsc = new HardStopCommand((MyThread)args[0]);
            var sd = new Mock<ISender>();
            sd.Setup(obj => obj.Send(It.IsAny<SpaceBattle.Lib.ICommand>())).Callback(() => q.Add(hsc));
            return sd.Object;
        }
    }

    class GetHardStop : IStrategy
    {
        public object run_strategy(params object[] args)
        {
            return new HardStopCommand((MyThread)args[0]);
        }
    }

    [Fact]
    public void SoftStopCommandTest()
    {
        var q = new BlockingCollection<SpaceBattle.Lib.ICommand>() { };
        var cmd1 = new Mock<SpaceBattle.Lib.ICommand>();
        var cmd2 = new Mock<SpaceBattle.Lib.ICommand>();
        cmd1.Setup(obj => obj.Execute()).Verifiable();
        var rc = new Mock<IReciever>();
        var fs = new FakeSender(q);
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Queue.Sender", (object[] args) => fs.run_strategy(args)).Execute();
        var mt = new MyThread(rc.Object);
        var sscmd = new SoftStopCommand(mt);
        cmd1.Setup(obj => obj.Execute()).Callback(() => q.Add(cmd2.Object)).Verifiable();
        rc.Setup(obj => obj.Recieve()).Returns(q.Take);
        var rg = new Mock<SpaceBattle.Lib.ICommand>();
        var hscmd = new GetHardStop();
        rg.Setup(obj => obj.Execute()).Callback(() =>
        {
            new InitScopeBasedIoCImplementationCommand().Execute();
            IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", ic).Execute();
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Commands.HardStop", (object[] args) => hscmd.run_strategy(args)).Execute();
        });
        var waiter = new AutoResetEvent(false);
        var smfr = new Mock<SpaceBattle.Lib.ICommand>();
        smfr.Setup(obj => obj.Execute()).Callback(() => waiter.Set());
        q.Add(rg.Object);
        q.Add(sscmd);
        q.Add(cmd1.Object);
        q.Add(smfr.Object);

        mt.Execute();

        waiter.WaitOne();
        cmd1.Verify(obj => obj.Execute());
    }

    void catchingMethod(SpaceBattle.Lib.ICommand c)
    {
        try
        {
            c.Execute();
            Assert.Fail("");
        }
        catch
        {
            Assert.Throws<Exception>(c.Execute);
        }
    }

    [Fact]
    public void wrongThreadHard()
    {
        var rc = new Mock<IReciever>();
        var mt = new MyThread(rc.Object);
        var hscmd = new HardStopCommand(mt);
        Thread t = new Thread(() => catchingMethod(hscmd));

        t.Start();
    }

    [Fact]
    public void wrongThreadSoft()
    {
        var sd = new Mock<ISender>();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Queue.Sender", (object[] args) => sd.Object).Execute();
        var rc = new Mock<IReciever>();
        var mt = new MyThread(rc.Object);
        var sscmd = new SoftStopCommand(mt);
        Thread t = new Thread(() => catchingMethod(sscmd));

        t.Start();
    }
}

