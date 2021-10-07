using Domain.Data;
using System;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Domain
{
	public class TelegramMessageHandler
	{
		private readonly DataProvider _dataProvider;
		private readonly ITelegramBotClient _telegramBotClient;
		private readonly RequestHandler _requestHandler;

		public TelegramMessageHandler(DataProvider dataProvider, ITelegramBotClient telegramBotClient, RequestHandler requestHandler)
		{
			_dataProvider = dataProvider ?? throw new ArgumentNullException(nameof(dataProvider));
			_telegramBotClient = telegramBotClient ?? throw new ArgumentNullException(nameof(telegramBotClient));
			_requestHandler = requestHandler ?? throw new ArgumentNullException(nameof(requestHandler));
		}

		public async Task HandleAsync(Update update)
		{
			if (update.Type != UpdateType.Message)
				return;

			if (update.Message.Type == MessageType.Text)
			{
				if (RequestService.TryGetRequestByMessage(update.Message.Text, out RequestType request))
				{
					Responce responce = _requestHandler.Handle(update, request);
					await _telegramBotClient.SendTextMessageAsync(update.Message.Chat.Id, responce.TextMessage, ParseMode.Markdown, replyMarkup: responce.Keyboard);
				}
				else
				{
					UserModel userModel = _dataProvider.GetUserModel(update.Message);
					if (userModel.Requests.Count == 0)
					{
						Responce responce = _requestHandler.Handle(update, RequestType.Startup);
						await _telegramBotClient.SendTextMessageAsync(update.Message.Chat.Id, responce.TextMessage, ParseMode.Markdown, replyMarkup: responce.Keyboard);
						return; 
					}

					switch (userModel.Requests[userModel.Requests.Count - 1])
					{
						case RequestType.None:
							break;
						case RequestType.Startup:
							break;
						case RequestType.CreateGroup:
							{
								_dataProvider.PartOnCreateGroup(userModel, update.Message.Text);
								userModel.Requests.Add(RequestType.InputGroupName);
								await _telegramBotClient.SendTextMessageAsync(update.Message.Chat.Id, "Введите дату по форме 27.5.2019 .");
								break;
							}
						case RequestType.InputGroupName:
							{
								if (DateTime.TryParse(update.Message.Text, out DateTime dateTime))
								{
									userModel.Requests.Add(RequestType.None);
									_dataProvider.PartOnCreateGroup(userModel, dateTime);
									await _telegramBotClient.SendTextMessageAsync(update.Message.Chat.Id, "Поздравляем. Группа успешно создана.");
									Responce responce = _requestHandler.Handle(update, RequestType.GroupMenu);
									await _telegramBotClient.SendTextMessageAsync(update.Message.Chat.Id, responce.TextMessage, ParseMode.Markdown, replyMarkup: responce.Keyboard);
								}
								else
									await _telegramBotClient.SendTextMessageAsync(update.Message.Chat.Id, "Дату нормально введи.");
								break;
							}
						case RequestType.JoinGroup:
							{
								Guid.TryParse(update.Message.Text, out Guid groupId);
								if (_dataProvider.TryToJoinGroup(userModel, groupId))
								{
									userModel.Requests.Add(RequestType.None);
									await _telegramBotClient.SendTextMessageAsync(update.Message.Chat.Id, "Поздравляем. Вы успешно присоединились к группе.");
									Responce responce = _requestHandler.Handle(update, RequestType.GroupMenu);
									await _telegramBotClient.SendTextMessageAsync(update.Message.Chat.Id, responce.TextMessage, ParseMode.Markdown, replyMarkup: responce.Keyboard);
								}
								else
									await _telegramBotClient.SendTextMessageAsync(update.Message.Chat.Id, "Не получилось присоединиться к группе.");
								break;
							}
						default:
							break;
					}
				}
			}
		}
	}
}
