namespace SpaceBattle.Lib;
using Hwdtech;

public class CommandSerializerStrategy : IStrategy
{
	public object run_strategy(params object[] argv)
	{
		CommandContract cc = (CommandContract)argv[0];
		var cmd = IoC.Resolve<SpaceBattle.Lib.ICommand>("Game.Commands.GetCommandByID", cc.game_item_id, cc.item_properties);
		var ret = new Dictionary<string, object>();
		ret.Add("game_id", cc.game_id);
		ret.Add("command", cmd);
		return(ret);
	}
}
