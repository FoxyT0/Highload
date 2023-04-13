namespace SpaceBattle.Lib.Test;

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

        var s = new Mock<ISender>();
        var GetSenderStrategy = new Mock<IStrategy>();
        GetExceptionStrategy.Setup(o => o.run_strategy()).Returns(s);
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Queue.Sender", (object[] args) => GetExceptionStrategy.Object.run_strategy()).Execute();
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
        Thread.Sleep(10);

        fakecmd.Verify(obj => obj.Execute());
    }
}

