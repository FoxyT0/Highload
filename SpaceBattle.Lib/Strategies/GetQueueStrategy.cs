
namespace SpaceBattle.Lib;

using Hwdtech;

public class GetQueue : IStrategy
{
	public object run_strategy(params object[] args)
	{
		string objId = (string)args[0];

		try
		{
			return IoC.Resolve<IDictionary<string, Queue<ICommand>>>("Game.Get.GameDictionary").TryGetValue(objId, out Queue<ICommand>? queue);
		}
		catch(KeyNotFoundException)
		{
			throw new Exception();
		}
	}
}

