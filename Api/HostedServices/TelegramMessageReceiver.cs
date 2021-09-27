using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;
using Telegram.Bot;
using Domain;

namespace Api.HostedServices
{
	public class TelegramMessageReceiver : BackgroundService
	{
		private readonly ITelegramBotClient _telegramBotClient;
		private readonly TelegramMessageHandler _telegramMessageHandler;

		public TelegramMessageReceiver(ITelegramBotClient telegramBotClient, TelegramMessageHandler telegramMessageHandler)
		{
			_telegramBotClient = telegramBotClient ?? throw new ArgumentNullException(nameof(telegramBotClient));
			_telegramMessageHandler = telegramMessageHandler ?? throw new ArgumentNullException(nameof(telegramMessageHandler));
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			int lastMessageId = 0;
			for (; !stoppingToken.IsCancellationRequested;)
			{
				Update[] updates = await _telegramBotClient.GetUpdatesAsync(
					offset: lastMessageId + 1,
					limit: 1,
					timeout: checked((int)_telegramBotClient.Timeout.TotalSeconds / 2),
					allowedUpdates: new UpdateType[] { UpdateType.Message },
					cancellationToken: stoppingToken
				);

				if (updates.Length == 0)
					continue;

				Update update = updates[0];
				lastMessageId = update.Id;
				try
				{
					await _telegramMessageHandler.HandleAsync(update);
				}
				catch (Exception exception)
				{
					Console.WriteLine(exception);
				}
			}
		}
	}
}
