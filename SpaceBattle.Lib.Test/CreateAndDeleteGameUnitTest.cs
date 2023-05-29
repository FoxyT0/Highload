namespace SpaceBattle.Lib.Test;

using Hwdtech;
using Hwdtech.Ioc;
using Moq;

public class CreateAndDeleteGameUnitTest
{
	Dictionary<string, object> scopes = new Dictionary<string, object>();
    public CreateAndDeleteGameUnitTest()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();
		IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set",  IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))).Execute();

		var GetScope = new Mock<IStrategy>();
		GetScope.Setup(o => o.run_strategy(It.IsAny<object[]>())).Returns((object[] args) => scopes[(string)args[0]]);
		var NewScopeToDict = new Mock<IStrategy>();
		NewScopeToDict.Setup(o => o.run_strategy(It.IsAny<object[]>())).Returns((object[] args) => {
				scopes.Add((string)args[0], IoC.Resolve<object>("Scopes.New", (object)args[1]));
				return scopes[(string)args[0]];
				});
		var DeleteScopeFromDict = new Mock<IStrategy>();
		DeleteScopeFromDict.Setup(o => o.run_strategy(It.IsAny<object[]>())).Returns((object[] args) => scopes.Remove((string)args[0]));
		
		IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Session.GetScope", (object[] args) => GetScope.Object.run_strategy(args)).Execute();
		IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Session.NewScope", (object[] args) => NewScopeToDict.Object.run_strategy(args)).Execute();
		IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Delete.New", (object[] args) => DeleteScopeFromDict.Object.run_strategy(args)).Execute();
		IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Create.New", (object[] args) => new CreateScopeAndGameStrategy().run_strategy(args)).Execute();
		
    }

	[Fact]
	public void GoodCreateTowGamesAndDeleteOneTest()
	{
		var gameCommand = IoC.Resolve<Mock<SpaceBattle.Lib.ICommand>>("Game.Create.New", "Game1", "Scope1");
		var gameCommand2 = IoC.Resolve<Mock<SpaceBattle.Lib.ICommand>>("Game.Create.New", "Game1", "Scope2");

		Assert.True(scopes.Count == 2);
		Assert.True(scopes["Scope1"] != scopes["Scope2"]);

		IoC.Resolve<bool>("Game.Delete.New", "Scope1");

		Assert.True(scopes.Count == 1);

		IoC.Resolve<bool>("Game.Delete.New", "Scope2");

		Assert.True(scopes.Count == 0);
	}

}
