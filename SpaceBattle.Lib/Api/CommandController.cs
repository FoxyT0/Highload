
namespace SpaceBattle.Lib;

using Microsoft.AspNetCore.Mvc;


[ApiController]
[Route("[controller]")]
public class SpaceBattleController : ControllerBase
{
	private readonly ThreadManager threadManager;

	public SpaceBattleController(ThreadManager threadManager)
	{
		this.threadManager = threadManager;
	}

	[HttpPost("/push")]
	public IActionResult PushCommand([FromHeader(Name = "Thread-Game-Id")] string tgid)
	{
		string[] parts = tgid.Split('.');
		ISender sender = threadManager.GetSender(parts[0].Trim());
		String rawJson = new StreamReader(Request.Body).ReadToEnd();
		ICommand cmd = new SendMsgToGameCommand(rawJson, tgid);
		sender.Send(cmd);

		return Ok();
	}
}

