
namespace SpaceBattle.Lib;

using Hwdtech;

public class GetIUObject : IStrategy
{
	public object run_strategy(params object[] args)
	{
		string objId = (string)args[0];

		try
		{
			return IoC.Resolve<IDictionary<string, IUObject>>("Game.Get.GameDictionary").TryGetValue(objId, out IUObject? queue);
		}
		catch(KeyNotFoundException)
		{
			throw new Exception();
		}
	}
}

