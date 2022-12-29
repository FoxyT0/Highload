namespace SpaceBattle.Lib.Test;

using Hwdtech;
using Hwdtech.Ioc;
using Moq;

class GetPropertyStrategy : IStrategy
{
    public object run_strategy(params object[] args)
    {
        string prop = (string)args[0];
        IUObject obj = (IUObject)args[1];
        return (obj.get_property(prop));
    }
}

class TreeTrue : IStrategy
{
    public object run_strategy(params object[] args)
    {
        return (true);
    }
}

class TreeFalse : IStrategy
{
    public object run_strategy(params object[] args)
    {
        return (false);
    }
}


public class CollisionCheckUnitTest
{
    public CollisionCheckUnitTest()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();
        IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))).Execute();

        var GetProperty = new GetPropertyStrategy();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.UObject.GetProperty", (object[] args) => GetProperty.run_strategy(args)).Execute();
    }
    [Fact]
    public void CollisionCheckTrue()
    {
        var TreeGivingTrueStrategy = new TreeTrue();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.DecisionTree.Collision", (object[] args) => TreeGivingTrueStrategy.run_strategy(args)).Execute();
        var first = new Mock<IUObject>();
        first.Setup(obj => obj.get_property("position")).Returns(new Vector(1, 1));
        first.Setup(obj => obj.get_property("velocity")).Returns(new Vector(2, 2));
        var second = new Mock<IUObject>();
        second.Setup(obj => obj.get_property("position")).Returns(new Vector(1, 2));
        second.Setup(obj => obj.get_property("velocity")).Returns(new Vector(3, 4));
        CollisionCheckCommand ccc = new CollisionCheckCommand(first.Object, second.Object);


        Assert.Throws<Exception>(() => ccc.Execute());
    }
    [Fact]
    public void CollisionCheckFalse()
    {
        var TreeGivingFalseStrategy = new TreeFalse();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.DecisionTree.Collision", (object[] args) => TreeGivingFalseStrategy.run_strategy(args)).Execute();
        var first = new Mock<IUObject>();
        first.Setup(obj => obj.get_property("position")).Returns(new Vector(1, 1));
        first.Setup(obj => obj.get_property("velocity")).Returns(new Vector(2, 2));
        var second = new Mock<IUObject>();
        second.Setup(obj => obj.get_property("position")).Returns(new Vector(1, 2));
        second.Setup(obj => obj.get_property("velocity")).Returns(new Vector(3, 4));
        CollisionCheckCommand ccc = new CollisionCheckCommand(first.Object, second.Object);

        ccc.Execute();

    }
}

