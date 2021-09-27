using System;
using System.Collections.Generic;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types;
using Domain.Data;

namespace Domain
{
	public class RequestHandler
	{
		private readonly DataProvider _dataProvider;
		private readonly KeyboardService _keyboardService;

		public RequestHandler(DataProvider dataProvider, KeyboardService keyboardService)
		{
			_dataProvider = dataProvider ?? throw new ArgumentNullException(nameof(dataProvider));
			_keyboardService = keyboardService ?? throw new ArgumentNullException(nameof(keyboardService));
		}

		private string GetGroupInfo(UserModel userModel)
		{
			string parity = userModel.Student.Group.Parity(_dataProvider.CorrectedDateTime) ? "числитель" : "знаменатель";
			DateTime startEducation = userModel.Student.Group.StartEducation;
			int weekCount = ((_dataProvider.CorrectedDateTime - startEducation).Days + ((int)startEducation.DayOfWeek - 1)) / 7 + 1;
			string result =
			"*Информация о группе:*\n" +
			"`Название: " + userModel.Student.Group.Name + ".\n" +
			"Начало семестра: " + userModel.Student.Group.StartEducation.ToShortDateString() + ".\n" +
			"Номер для приглашения: " + userModel.Student.Group.Id + ".\n" +
			weekCount + " неделя " + parity + ".`";
			return result;
		}

		public Responce Handle(Update update, RequestType request)
		{
			UserModel userModel = _dataProvider.GetUserModel(update.Message);
			Responce responce;
			switch (request)
			{
				case RequestType.Startup:
					{
						responce = userModel.Student.Group == null
							? new Responce(_keyboardService.GetKeyboardByRequest(request), "Стартовое меню.")
							: new Responce(_keyboardService.GetKeyboardByRequest(RequestType.GroupMenu), GetGroupInfo(userModel));
						break;
					}
				case RequestType.Backward:
					{
						return Handle(update, userModel.Requests[userModel.Requests.Count - 2]);
					}
				case RequestType.CreateGroup:
					{
						userModel.Requests.Add(RequestType.CreateGroup);
						responce = new Responce(_keyboardService.GetKeyboardByRequest(request), "Введите название группы: .");
						break;
					}
				case RequestType.JoinGroup:
					{
						userModel.Requests.Add(RequestType.JoinGroup);
						responce = new Responce(_keyboardService.GetKeyboardByRequest(request), "Введите номер группы: .");
						break;
					}
				case RequestType.GroupMenu:
					responce = new Responce(_keyboardService.GetKeyboardByRequest(request), GetGroupInfo(userModel));
					break;
				case RequestType.LeaveGroup:
					{
						_dataProvider.LeaveGroup(userModel);
						responce = new Responce(_keyboardService.GetKeyboardByRequest(RequestType.Startup), "Стартовое меню.");
						break;
					}
				case RequestType.WatchFullSchedule:
					{
						responce = _dataProvider.GetFullShedule(userModel, out string schedule)
							? new Responce(_keyboardService.GetKeyboardByRequest(RequestType.Backward), schedule)
							: new Responce(_keyboardService.GetKeyboardByRequest(RequestType.Backward), "Расписание не достпуно.");
						break;
					}
				case RequestType.WatchScheduleOnTomorrow:
					{
						responce = _dataProvider.GetSheduleOnTomorrow(userModel, out string schedule)
							? new Responce(_keyboardService.GetKeyboardByRequest(RequestType.Backward), schedule)
							: new Responce(_keyboardService.GetKeyboardByRequest(RequestType.Backward), "Расписание не достпуно.");
						break;
					}
				case RequestType.WatchScheduleOnToday:
					{
						responce = _dataProvider.GetSheduleOnToday(userModel, out string schedule)
							? new Responce(_keyboardService.GetKeyboardByRequest(RequestType.Backward), schedule)
							: new Responce(_keyboardService.GetKeyboardByRequest(RequestType.Backward), "Расписание не достпуно.");
						break;
					}
				default:
					responce = new Responce(_keyboardService.GetKeyboardByRequest(RequestType.None), "Введите /start");
					break;
			}
			userModel.Requests.Add(request);
			return responce;
		}
	}
	public struct Responce
	{
		public ReplyMarkupBase Keyboard;
		public string TextMessage;

		public Responce(ReplyMarkupBase keyboard, string textMessage)
		{
			Keyboard = keyboard ?? throw new ArgumentNullException(nameof(keyboard));
			TextMessage = textMessage ?? throw new ArgumentNullException(nameof(textMessage));
		}
	}

	public class KeyboardService
	{
		private enum Keyboard { None, Backward, StartMenu, CreateGroup, GroupMenu }

		private Dictionary<Keyboard, ReplyMarkupBase> _keyboards;

		public KeyboardService()
		{
			_keyboards = new Dictionary<Keyboard, ReplyMarkupBase>
			{
				{ Keyboard.None, new ReplyKeyboardRemove() { Selective = true } }
			};

			ReplyKeyboardMarkup backward = new ReplyKeyboardMarkup
			{
				Keyboard = new KeyboardButton[][]
				{
					new KeyboardButton[]
					{
						new KeyboardButton(RequestService.GetMessageByRequest(RequestType.Backward)),
					},
				}
			};
			backward.ResizeKeyboard = true;
			_keyboards.Add(Keyboard.Backward, backward);
			ReplyKeyboardMarkup startMenu = new ReplyKeyboardMarkup
			{
				Keyboard = new KeyboardButton[][]
				{
					new KeyboardButton[]
					{
						new KeyboardButton(RequestService.GetMessageByRequest(RequestType.CreateGroup)),
					},

					new KeyboardButton[]
					{
						new KeyboardButton(RequestService.GetMessageByRequest(RequestType.JoinGroup))
					},
				}
			};
			startMenu.ResizeKeyboard = true;
			_keyboards.Add(Keyboard.StartMenu, startMenu);
			ReplyKeyboardMarkup createGroupMenu = new ReplyKeyboardMarkup
			{
				Keyboard = new KeyboardButton[][]
				{
					new KeyboardButton[]
					{
						new KeyboardButton(RequestService.GetMessageByRequest(RequestType.CreateGroup)),
					},
				}
			};
			createGroupMenu.ResizeKeyboard = true;
			_keyboards.Add(Keyboard.CreateGroup, createGroupMenu);
			ReplyKeyboardMarkup groupMenu = new ReplyKeyboardMarkup
			{
				Keyboard = new KeyboardButton[][]
				{
					new KeyboardButton[]
					{
						new KeyboardButton(RequestService.GetMessageByRequest(RequestType.LeaveGroup)),
					},
					new KeyboardButton[]
					{
						new KeyboardButton(RequestService.GetMessageByRequest(RequestType.WatchFullSchedule)),
					},
					new KeyboardButton[]
					{
						new KeyboardButton(RequestService.GetMessageByRequest(RequestType.WatchScheduleOnTomorrow)),
					},
					new KeyboardButton[]
					{
						new KeyboardButton(RequestService.GetMessageByRequest(RequestType.WatchScheduleOnToday)),
					},
				}
			};
			groupMenu.ResizeKeyboard = true;
			_keyboards.Add(Keyboard.GroupMenu, groupMenu);
		}

		public ReplyMarkupBase GetKeyboardByRequest(RequestType request)
		{
			return request switch
			{
				RequestType.Startup => _keyboards.GetValueOrDefault(Keyboard.StartMenu),
				RequestType.Backward => _keyboards.GetValueOrDefault(Keyboard.Backward),
				RequestType.CreateGroup => _keyboards.GetValueOrDefault(Keyboard.None),
				RequestType.JoinGroup => _keyboards.GetValueOrDefault(Keyboard.None),
				RequestType.GroupMenu => _keyboards.GetValueOrDefault(Keyboard.GroupMenu),
				_ => _keyboards.GetValueOrDefault(Keyboard.None),
			};
		}
	}
}
