using Domain.Data;
using System;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Domain
{
	public class TelegramMessageHandler
	{
		private readonly DataProvider _dataProvider;
		private readonly RequestRecognizer _requestRecognizer;
		private readonly TelegramMessageSender _telegramMessageSender;

		public TelegramMessageHandler(DataProvider dataProvider, RequestRecognizer requestRecognizer, TelegramMessageSender telegramMessageSender)
		{
			_dataProvider = dataProvider ?? throw new ArgumentNullException(nameof(dataProvider));
			_requestRecognizer = requestRecognizer ?? throw new ArgumentNullException(nameof(requestRecognizer));
			_telegramMessageSender = telegramMessageSender ?? throw new ArgumentNullException(nameof(telegramMessageSender));
		}

		public async Task HandleAsync(Update update)
		{
			if (update.Type != UpdateType.Message && update.Message.Type != MessageType.Text)
				return;

			UserModel userModel = _dataProvider.GetUserModel(update.Message);

			if (_requestRecognizer.TryGetRequestTypeByMessage(update.Message.Text, out RequestType request))
			{
				await _telegramMessageSender.SendMessage(userModel, request);
			}
			else
			{
				if (userModel.Requests.Count == 0)
				{
					await _telegramMessageSender.SendMessage(userModel, RequestType.Startup);
					return;
				}

				switch (userModel.Requests[userModel.Requests.Count - 1])
				{
					case RequestType.CreateGroup:
						{
							_dataProvider.PartOnCreateGroup(userModel, update.Message.Text);
							userModel.Requests.Add(RequestType.InputGroupName);
							await _telegramMessageSender.SendDateInputRequest(userModel);
							break;
						}
					case RequestType.InputGroupName:
						{
							if (DateTime.TryParse(update.Message.Text, out DateTime dateTime))
							{
								userModel.Requests.Add(RequestType.None);
								_dataProvider.PartOnCreateGroup(userModel, dateTime);
								await _telegramMessageSender.SendCreationGroupCongratulation(userModel);
							}
							else
								await _telegramMessageSender.SendDateInputError(userModel);
							break;
						}
					case RequestType.JoinGroup:
						{
							Guid.TryParse(update.Message.Text, out Guid groupId);
							if (_dataProvider.TryToJoinGroup(userModel, groupId))
							{
								userModel.Requests.Add(RequestType.None);
								await _telegramMessageSender.SendJoinToGroupCongratulation(userModel);
							}
							else
								await _telegramMessageSender.SendJoinToGroupException(userModel);
							break;
						}
					default:
						break;
				}
			}
		}
	}
}
