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

public class CollisionCheckUnitTest
{
    public CollisionCheckUnitTest()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();
        IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))).Execute();

        var GetProperty = new GetPropertyStrategy();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.UObject.GetProperty", (object[] args) => GetProperty.run_strategy(args)).Execute();

        var NodeT = new TrueNode<Vector>();
        var NodeF = new FalseNode<Vector>();
        var NodeL = new CollisionNode<Vector>(NodeF, NodeT, 1, new Vector(1, 0));
        var NodeR = new CollisionNode<Vector>(NodeF, NodeT, 1, new Vector(0, 1));
        var Root = new CollisionNode<Vector>(NodeL, NodeR, 0, new Vector(1, 1));
        var GetTreeStrategy = new Mock<IStrategy>();
        GetTreeStrategy.Setup(o => o.run_strategy()).Returns(Root);
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Trees.Collision", (object[] args) => GetTreeStrategy.Object.run_strategy(args)).Execute();

        var GetDecisionStrategy = new CollisionCheckStrategy();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.DecisionTree.Collision", (object[] args) => GetDecisionStrategy.run_strategy(args)).Execute();
    }
    [Fact]
    public void CollisionCheckFalse()
    {
        var first = new Mock<IUObject>();
        first.Setup(obj => obj.get_property("position")).Returns(new Vector(1, 1));
        first.Setup(obj => obj.get_property("velocity")).Returns(new Vector(2, 2));
        var second = new Mock<IUObject>();
        second.Setup(obj => obj.get_property("position")).Returns(new Vector(1, 1));
        second.Setup(obj => obj.get_property("velocity")).Returns(new Vector(2, 2));
        CollisionCheckCommand ccc = new CollisionCheckCommand(first.Object, second.Object);

        ccc.Execute();

    }
    [Fact]
    public void CollisionCheckTrue()
    {
        var first = new Mock<IUObject>();
        first.Setup(obj => obj.get_property("position")).Returns(new Vector(1, 1));
        first.Setup(obj => obj.get_property("velocity")).Returns(new Vector(3, 2));
        var second = new Mock<IUObject>();
        second.Setup(obj => obj.get_property("position")).Returns(new Vector(1, 1));
        second.Setup(obj => obj.get_property("velocity")).Returns(new Vector(2, 2));
        CollisionCheckCommand ccc = new CollisionCheckCommand(first.Object, second.Object);


        Assert.Throws<Exception>(() => ccc.Execute());
    }
}

