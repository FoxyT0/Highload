namespace SpaceBattle.Lib;

public class HardStopThreadStrategy
{
    public object run_strategy(params object[] args)
    {
        string id = (string)args[0];
        Action act = () => { };
        if (args[1] != null)
        {
            act = (Action)args[1];
        }
        var cmd = new HardStopThreadCommand(id, act);
        return cmd;
    }
}
