using Data;
using Database.Data;
using Database.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Domain.Data
{
	public class DataProvider
	{
		private const int _timeDifferent = 3;

		private readonly DataContext _dataContext;
		private readonly ITelegramBotClient _telegramBotClient;

		private List<SubjectTimeSlot> _subjectCalls;
		private Dictionary<long, UserModel> _cachedUserModels;
		private Dictionary<UserModel, string> _creationGroup;

		public DataProvider(DataContext dataContext, ITelegramBotClient telegramBotClient)
		{
			_dataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));
			_telegramBotClient = telegramBotClient ?? throw new ArgumentNullException(nameof(telegramBotClient));

			_dataContext.Database.EnsureCreated();
			_dataContext.LoadAll();


			_subjectCalls = _dataContext.SubjectTimeSlots.ToList();
			_subjectCalls.Sort(new Comparison<SubjectTimeSlot>((x, y) => x.Order.CompareTo(y.Order)));
			_cachedUserModels = new Dictionary<long, UserModel>();
			_creationGroup = new Dictionary<UserModel, string>();
		}

		public DateTime CorrectedDateTime => DateTime.Now.AddHours(_timeDifferent);
		public List<SubjectTimeSlot> SubjectCalls => _subjectCalls;

		public void AddLog(string log) => _dataContext.AddLog(log);
		public UserModel GetUserModel(Message message)
		{
			if (_cachedUserModels.TryGetValue(message.From.Id, out UserModel userModel))
				return userModel;

			if (!_dataContext.TryGetStudentByTelegramId(message.From.Id, out Student student))
			{
				_dataContext.CreateStudent(message.From.Id, message.Chat.Id, message.From.Username + " (" + message.From.FirstName + " " + message.From.LastName + ")", out student);
				_dataContext.SaveChanges();
			}

			UserModel result = new()
			{
				Student = student,
				User = message.From
			};
			_cachedUserModels.Add(message.From.Id, result);
			return result;
		}
		public void PartOnCreateGroup(UserModel userModel, string name)
		{
			_creationGroup.Remove(userModel);
			_creationGroup.Add(userModel, name);
		}
		public void PartOnCreateGroup(UserModel userModel, DateTime startEducation)
		{
			if (_creationGroup.TryGetValue(userModel, out string name))
			{
				_dataContext.CreateGroup(name, startEducation, userModel.Student, out Group group);
				userModel.Student.Group = group;
				_dataContext.SaveChanges();
				_creationGroup.Remove(userModel);
			}
		}
		public bool TryToJoinGroup(UserModel userModel, Guid groupId)
		{
			Group group = _dataContext.Groups.FirstOrDefault(x => x.Id == groupId);
			if (group == null)
				return false;
			userModel.Student.Group = group;
			_dataContext.SaveChanges();
			return true;
		}
		public void LeaveGroup(UserModel userModel)
		{
			userModel.Student.Group = null;
			_dataContext.SaveChanges();
		}
		public bool GetFullShedule(UserModel userModel, out string schedule)
		{
			schedule = default;
			Group group = _dataContext.Groups.FirstOrDefault(x => x.Id == userModel.Student.Group.Id);
			return group == null ? false : ScheduleViewReader.GetSchedule(group, out schedule);
		}
		public bool GetSheduleOnTomorrow(UserModel userModel, out string schedule)
		{
			schedule = default;
			Group group = _dataContext.Groups.FirstOrDefault(x => x.Id == userModel.Student.Group.Id);
			return group == null ? false : ScheduleViewReader.GetSchedule(userModel.Student.Group, CorrectedDateTime.AddDays(1), _subjectCalls, out schedule);
		}
		public bool GetSheduleOnToday(UserModel userModel, out string schedule)
		{
			schedule = default;
			Group group = _dataContext.Groups.FirstOrDefault(x => x.Id == userModel.Student.Group.Id);
			return group == null ? false : ScheduleViewReader.GetSchedule(userModel.Student.Group, CorrectedDateTime, _subjectCalls, out schedule);
		}

		public void StartSubjectDateTimeNotificate(int order)
		{
			foreach (Group group in _dataContext.Groups)
			{
				List<ScheduleView> schedule = ScheduleViewReader.GetSchedule(group, CorrectedDateTime.DayOfWeek, group.Parity(CorrectedDateTime));
				if (schedule.Select(x => x.Order).Min() == order)
				{
					ScheduleView subject = schedule.First(x => x.Order == order);
					foreach (Student user in group.Students)
						_telegramBotClient.SendTextMessageAsync(user.ChatId, "`Первая пара " + subject.SubjectInstance.Subject.Name + " " + ScheduleViewReader.GetSubjectTypeString(subject.SubjectInstance.SubjectType) + " в " + subject.SubjectInstance.Audience + " аудитории. Ведёт " + subject.SubjectInstance.Teacher + "`.", Telegram.Bot.Types.Enums.ParseMode.Markdown);
				}
			}
		}
		public void EndSubjectDateTimeNotificate(int order)
		{
			foreach (Group group in _dataContext.Groups)
			{
				List<ScheduleView> schedule = ScheduleViewReader.GetSchedule(group, CorrectedDateTime.DayOfWeek, group.Parity(CorrectedDateTime));
				if (schedule.Select(x => x.Order).Contains(order))
				{
					ScheduleView subject = schedule.First(x => x.Order == order);
					foreach (Student user in group.Students)
					{
						ScheduleView nextSubject = schedule.FirstOrDefault(z => z.Order == order + 1);
						if (nextSubject != null)
							_telegramBotClient.SendTextMessageAsync(user.ChatId, "`Пара " + subject.SubjectInstance.Subject.Name + " закончилась. Следующая пара " + nextSubject.SubjectInstance.Subject.Name + " " + ScheduleViewReader.GetSubjectTypeString(nextSubject.SubjectInstance.SubjectType) + " в " + nextSubject.SubjectInstance.Audience + " аудитории. Ведёт " + nextSubject.SubjectInstance.Teacher + "`.", Telegram.Bot.Types.Enums.ParseMode.Markdown);
						else
							_telegramBotClient.SendTextMessageAsync(user.ChatId, "`Пара " + subject.SubjectInstance.Subject.Name + " закончилась. Теперь домой!`", Telegram.Bot.Types.Enums.ParseMode.Markdown);
					}
				}
			}
		}
	}
	public class UserModel
	{
		public Student Student { get; set; }
		public User User { get; set; }
		public List<RequestType> Requests { get; } = new List<RequestType>();
	}
}
