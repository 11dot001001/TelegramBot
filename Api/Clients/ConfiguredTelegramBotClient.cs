using Microsoft.Extensions.Configuration;
using Telegram.Bot;

namespace Api.Clients
{
	public class ConfiguredTelegramBotClient : TelegramBotClient
	{
		public ConfiguredTelegramBotClient(IConfiguration configuration) : base(configuration["TelegramToken"])
		{
			
		}
	}
}
