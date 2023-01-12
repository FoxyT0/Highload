namespace SpaceBattle.Lib;

public class CollisionNode<T> : INode<T> where T : Vector
{
    INode<T> left;
    INode<T> right;
    int prop;
    T check;
    public CollisionNode(INode<T> l, INode<T> r, int p, T c)
    {
        this.left = l;
        this.right = r;
        this.prop = p;
        this.check = c;
    }
    public bool decision(IList<T> args)
    {
        if (args[prop] < check)
        {
            return (left.decision(args));
        }
        return (right.decision(args));
    }
}

