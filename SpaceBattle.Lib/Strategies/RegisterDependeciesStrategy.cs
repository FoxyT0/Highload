namespace SpaceBattle.Lib;

using Hwdtech;
using Moq;

public class RegisterDependenciesStrategy : IStrategy
{
	public object run_strategy(params object[] args)
	{
		object currScope = (object)args[0];

		IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", currScope).Execute();
		
		return new ActionCommand(()=> {
				Dictionary<string, IUObject> ships = new Dictionary<string, IUObject>();
				Queue<SpaceBattle.Lib.ICommand> queue = new Queue<SpaceBattle.Lib.ICommand>();
				var TimeSpanGame = new Mock<IStrategy>();
				TimeSpanGame.Setup(o => o.run_strategy(It.IsAny<object[]>())).Returns((object[] args) => new TimeSpan(100));
				var GetObjcet = new Mock<IStrategy>();
				GetObjcet.Setup(o => o.run_strategy(It.IsAny<object[]>())).Returns((object[] args) => ships[(string)args[0]]);
				var DeleteObject = new Mock<IStrategy>();
				DeleteObject.Setup(o => o.run_strategy(It.IsAny<object[]>())).Returns((object[] args) => ships.Remove((string)args[0]));
				var EnqueueCommand = new Mock<IStrategy>();
				EnqueueCommand.Setup(o => o.run_strategy(It.IsAny<object[]>())).Returns((object[] args) => new ActionCommand(() => queue.Enqueue((SpaceBattle.Lib.ICommand)args[0])));
				var DequeueCommand = new Mock<IStrategy>();
				DequeueCommand.Setup(o => o.run_strategy(It.IsAny<object[]>())).Returns((object[] args) => queue.Dequeue());
				});
	}
}
