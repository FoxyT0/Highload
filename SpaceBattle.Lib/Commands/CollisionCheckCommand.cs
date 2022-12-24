namespace SpaceBattle.Lib;

using Hwdtech;

public class CollisionCheckCommand : ICommand
{
    private IUObject firstTarget;
    private IUObject secondTarget;

    public CollisionCheckCommand(IUObject firstObject, IUObject secondObject)
    {
        this.firstTarget = firstObject;
        this.secondTarget = secondObject;
    }

    public void Execute()
    {
        bool areCollided = IoC.Resolve<bool>("Game.DecisionTree.Collision", firstTarget, secondTarget);

        if (areCollided)
        {
            throw (new Exception("Objects are collided"));
        }
    }
}
