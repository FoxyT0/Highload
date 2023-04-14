namespace SpaceBattle.Lib.Test;

using System.Collections.Concurrent;
using Hwdtech;
using Hwdtech.Ioc;
using Moq;

public class MyThreadUnitTests
{
    public MyThreadUnitTests()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();
        IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))).Execute();

        var ex = new Exception();
        var GetExceptionStrategy = new Mock<IStrategy>();
        GetExceptionStrategy.Setup(o => o.run_strategy()).Returns(ex);
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Exceptions.StoppingWrongThread", (object[] args) => GetExceptionStrategy.Object.run_strategy()).Execute();
    }

    [Fact]
    public void RecieverAdapterTake()
    {
        var cmd = new Mock<SpaceBattle.Lib.ICommand>();
        cmd.Setup(obj => obj.Execute()).Verifiable();
        var q = new BlockingCollection<SpaceBattle.Lib.ICommand>() { cmd.Object };
        var ra = new RecieverAdapter(q);

        q.Take().Execute();

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
        var cmd = new Mock<SpaceBattle.Lib.ICommand>();
        cmd.Setup(obj => obj.Execute()).Verifiable();
        var rc = new Mock<IReciever>();
        rc.Setup(obj => obj.Recieve()).Returns(cmd.Object);
        var mt = new MyThread(rc.Object);

        mt.Execute();
        Thread.Sleep(10);

        cmd.Verify(obj => obj.Execute());
    }

    [Fact]
    public void UpdateBehaviorTest()
    {
        var cmd = new Mock<SpaceBattle.Lib.ICommand>();
        cmd.Setup(obj => obj.Execute());
        var rc = new Mock<IReciever>();
        rc.Setup(obj => obj.Recieve()).Returns(cmd.Object);
        var fakecmd = new Mock<SpaceBattle.Lib.ICommand>();
        fakecmd.Setup(obj => obj.Execute()).Verifiable();
        var newbhvr = new Action(() => fakecmd.Object.Execute());
        var mt = new MyThread(rc.Object);
        var ubcmd = new UpdateBehaviorCommand(mt, newbhvr);

        ubcmd.Execute();
        mt.Execute();
        Thread.Sleep(1);

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
        Thread.Sleep(10);

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
        q.Add(sscmd);
        q.Add(cmd1.Object);

        mt.Execute();
        Thread.Sleep(10);

        cmd1.Verify(obj => obj.Execute());
        cmd2.Verify(obj => obj.Execute(), Times.Never());
    }
}

