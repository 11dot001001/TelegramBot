using Data;
using Database.Data.Model;
using Domain.Data;
using System;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace Domain
{
	public class TelegramMessageSender
	{
		private readonly ITelegramBotClient _telegramBotClient;
		private readonly RequestHandler _requestHandler;

		public TelegramMessageSender(ITelegramBotClient telegramBotClient, RequestHandler requestHandler)
		{
			_telegramBotClient = telegramBotClient ?? throw new ArgumentNullException(nameof(telegramBotClient));
			_requestHandler = requestHandler ?? throw new ArgumentNullException(nameof(requestHandler));
		}

		public Task NotifyFirstSubjectStart(long chatId, SubjectInstance subjectInstance)
		{
			return _telegramBotClient.SendTextMessageAsync(
				chatId,
				"`Первая пара " + subjectInstance.Subject.Name + " " +
				ScheduleViewReader.GetSubjectTypeString(subjectInstance.SubjectType) +
				" в " + subjectInstance.Audience + " аудитории. " +
				"Ведёт " + subjectInstance.Teacher + "`.",
				ParseMode.Markdown
			);
		}
		public Task NotifySubjectEnd(long chatId, SubjectInstance subjectInstance, SubjectInstance nextSubjectInstance)
		{
			return _telegramBotClient.SendTextMessageAsync(
				chatId,
				"`Пара " + subjectInstance.Subject.Name + " закончилась. " +
				"Следующая пара " + nextSubjectInstance.Subject.Name + " " +
				ScheduleViewReader.GetSubjectTypeString(nextSubjectInstance.SubjectType) +
				" в " + nextSubjectInstance.Audience + " аудитории. " +
				"Ведёт " + nextSubjectInstance.Teacher + "`.",
				ParseMode.Markdown
			);
		}
		public Task NotifySubjectEnd(long chatId, SubjectInstance subjectInstance)
		{
			return _telegramBotClient.SendTextMessageAsync(
				chatId,
				"`Пара " + subjectInstance.Subject.Name + " закончилась. Теперь домой!`",
				ParseMode.Markdown
			);
		}

		public Task SendMessage(UserModel userModel, RequestType requestType)
		{
			Responce responce = _requestHandler.Handle(userModel, requestType);
			return _telegramBotClient.SendTextMessageAsync(
				userModel.ChatId,
				responce.TextMessage,
				ParseMode.Markdown,
				replyMarkup: responce.Keyboard
			);
		}

		public async Task SendJoinToGroupCongratulation(UserModel userModel)
		{
			await _telegramBotClient.SendTextMessageAsync(userModel.ChatId, "Поздравляем. Вы успешно присоединились к группе.");
			await SendMessage(userModel, RequestType.GroupMenu);
		}
		public Task SendJoinToGroupException(UserModel userModel)
		{
			return _telegramBotClient.SendTextMessageAsync(userModel.ChatId, "Не получилось присоединиться к группе.");
		}

		public async Task SendCreationGroupCongratulation(UserModel userModel)
		{
			await _telegramBotClient.SendTextMessageAsync(userModel.ChatId, "Поздравляем. Группа успешно создана.");
			await SendMessage(userModel, RequestType.GroupMenu);
		}

		public Task SendDateInputRequest(UserModel userModel)
		{
			return _telegramBotClient.SendTextMessageAsync(userModel.ChatId, "Введите дату по форме 27/5/2021 .");
		}
		public Task SendDateInputError(UserModel userModel)
		{
			return _telegramBotClient.SendTextMessageAsync(userModel.ChatId, "Ошибка ввода даты.");
		}
	}
}
