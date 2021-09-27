using Microsoft.AspNetCore.Mvc;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace Api.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class TelegramController : ControllerBase
	{
		[HttpPost]
		public Task<int> SendUpdateAsync(Update update)
		{
			Console.WriteLine(update.Id + " " + update.Message.Text);
			return Task.FromResult(12);
		}
	}
}
