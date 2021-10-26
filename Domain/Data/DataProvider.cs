using Data;
using Database.Data;
using Database.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using Telegram.Bot.Types;

namespace Domain.Data
{
	public class DataProvider
	{
		private const int MoscowTimezone = 3;

		private readonly DataContext _dataContext;

		private readonly List<SubjectTimeSlot> _subjectCalls;
		private readonly Dictionary<long, UserModel> _cachedUserModels;
		private readonly Dictionary<UserModel, string> _creationGroup;

		public DataProvider(DataContext dataContext)
		{
			_dataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));

			_dataContext.Database.EnsureCreated();
			_dataContext.LoadAll();

			_subjectCalls = _dataContext.SubjectTimeSlots.ToList();
			_subjectCalls.Sort(new Comparison<SubjectTimeSlot>((x, y) => x.Order.CompareTo(y.Order)));
			_cachedUserModels = new Dictionary<long, UserModel>();
			_creationGroup = new Dictionary<UserModel, string>();
		}

		public DateTime MoscowDateTime => DateTime.UtcNow.AddHours(MoscowTimezone);

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
				User = message.From,
				ChatId = message.Chat.Id
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
			return group == null ? false : ScheduleViewReader.GetSchedule(userModel.Student.Group, MoscowDateTime.AddDays(1), _subjectCalls, out schedule);
		}
		public bool GetSheduleOnToday(UserModel userModel, out string schedule)
		{
			schedule = default;
			Group group = _dataContext.Groups.FirstOrDefault(x => x.Id == userModel.Student.Group.Id);
			return group == null ? false : ScheduleViewReader.GetSchedule(userModel.Student.Group, MoscowDateTime, _subjectCalls, out schedule);
		}
	}
	public class UserModel
	{
		public Student Student { get; set; }
		public User User { get; set; }
		public long ChatId { get; set; }
		public List<RequestType> Requests { get; } = new List<RequestType>();
	}
}
