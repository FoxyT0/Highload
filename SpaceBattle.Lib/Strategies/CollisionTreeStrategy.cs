namespace SpaceBattle.Lib;

using Hwdtech;

public class CollisionCheckStrategy : IStrategy
{
    public object run_strategy(params object[] args)
    {
        var l = new List<Vector>();
        foreach (Vector arg in args)
        {
            l.Add(arg);
        }
        var tree = IoC.Resolve<INode<Vector>>("Game.Trees.Collision");
        return tree.decision(l);
    }
}
