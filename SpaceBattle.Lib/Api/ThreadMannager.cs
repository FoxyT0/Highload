namespace SpaceBattle.Lib;

using Hwdtech;

public class ThreadManager
{
	private Dictionary<string, ISender> threadSenders = new Dictionary<string, ISender>();

	public ThreadManager(int threadsCount, int gameCount)
	{
		int threadsCount = threadsCount;
        int gameCount = gameCount;

		for(int i = 0; i < threadsCount; i++) {
			String ThreadId = i.ToString();
			Action start = () =>
			{
				ISender gameSender = IoC.Resolve<ISender>("Game.Threads.GetInnerSender", id);
				for(int j = 1; j <= gameNum; j++) {
                    string gameId = j.toString(); 
                    object gameScope = IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))
					IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", gameScope).Execute();

					String gtid = ThreadId + gameId;			
					ICommand gcmd = IoC.Resolve<ICommand>("Game.Session.Create", tgid, tgid);
					gameSender.Send(gcmd);
				};
			};
			IoC.Resolve<ICommand>("Game.Threads.Start", ThreadId, start).Execute();
			ISender sender = IoC.Resolve<ISender>("Game.Threads.GetOuterSender", ThreadId);
			threadSenders[ThreadId] = sender;
		};
	}

	public ISender GetSender(String ThreadId)
	{
		return threadSenders[ThreadId];
	}
}

