namespace SpaceBattle.Lib;

using Hwdtech;

public class SendMsgToGameCommand : ICommand
{
	private String gameId;
	private String msg;

	public SendMsgToGameCommand(String msg, String gameId)
	{
		this.msg = msg;
		this.gameId = gameId;
	}

	public void Execute()
	{
			ICommand command = IoC.Resolve<ICommand>("Game.Commands.Deserialize", msg);
			
            object scope = IoC.Resolve<object>("Game.Sessions.Scope", gameId);
			IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", scope).Execute();

			IoC.Resolve<ICommand>("Queue.Enqueue.Command", command).Execute();
	}
}