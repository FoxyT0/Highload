namespace SpaceBattle.Lib.Test;

using Hwdtech;
using Hwdtech.Ioc;
using Moq;

class TreeGivingTrue : IStrategy
{
    public object run_strategy(params object[] args)
    {
        return true;
    }
}

class TreeGivingFalse : IStrategy
{
    public object run_strategy(params object[] args)
    {
        return false;
    }
}

public sealed class AssertEx
{
    public static void NoExceptionThrown(Action a)
    {
        try
        {
            a();
        }
        catch (Exception)
        {
            Assert.Fail("Expected no exceptions to be thrown");
        }
    }
}

public class CollisionCheckUnitTest
{
    public CollisionCheckUnitTest()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();
        IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))).Execute();
    }
    [Fact]
    public void CollisionCheckTrue()
    {
        var TreeGivingTrueStrategy = new TreeGivingTrue();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.DecisionTree.Collision", (object[] args) => TreeGivingTrueStrategy.run_strategy(args)).Execute();
        var first = new Mock<IUObject>();
        var second = new Mock<IUObject>();
        CollisionCheckCommand ccc = new CollisionCheckCommand(first.Object, second.Object);


        Assert.Throws<Exception>(() => ccc.Execute());
    }
    [Fact]
    public void CollisionCheckFalse()
    {
        var TreeGivingFalseStrategy = new TreeGivingFalse();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.DecisionTree.Collision", (object[] args) => TreeGivingFalseStrategy.run_strategy(args)).Execute();
        var first = new Mock<IUObject>();
        var second = new Mock<IUObject>();
        CollisionCheckCommand ccc = new CollisionCheckCommand(first.Object, second.Object);


        AssertEx.NoExceptionThrown(() => ccc.Execute());
    }
}

