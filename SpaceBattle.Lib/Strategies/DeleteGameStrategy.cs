namespace SpaceBattle.Lib;

using Hwdtech;

public class DeleteGameStrategy : IStrategy
{
	public object run_strategy(params object[] args)
	{
		string scopeId = (string)args[0];

		object rootScope = IoC.Resolve<object>("Scopes.Current");
		
					

		return 1;
	}
}
