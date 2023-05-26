namespace SpaceBattle.Lib;

public interface IMessage
{
	public string cmd {get;}
	public int gameId {get;}
	public int objId {get;}
	public IDictionary<string, object> properties {get;}
}
