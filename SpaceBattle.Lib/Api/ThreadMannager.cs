namespace SpaceBattle.Lib;

using Hwdtech;

public class ThreadManager
{
	private Dictionary<string, ISender> threadSenders = new Dictionary<string, ISender>();

    private int threads;
    private int games;    

	public ThreadManager(int threads, int games)
	{
		this.threads = threads;
        this.games = games;

		for(int i = 0; i < threads; i++) {
			String ThreadId = i.ToString();
			Action start = () =>
			{
				ISender gameSender = IoC.Resolve<ISender>("Game.Threads.GetInnerSender", ThreadId);
				for(int j = 1; j <= games; j++) {
                    string gameId = j.ToString(); 
                    object gameScope = IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"));
					IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", gameScope).Execute();

					String tgid = ThreadId + "." + gameId;			
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


