using Data;
using Database.Data;
using Database.Data.Model;
using Domain.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Domain
{
	public class ScheduleNotifier
	{
		private class DailyNotification
		{
			public DailyNotification(DateTime dateTime, int order)
			{
				DateTime = dateTime;
				TimeSlotOrder = order;
			}

			public DateTime DateTime { get; set; }
			public int TimeSlotOrder { get; set; }
			public bool IsNotified { get; set; }
		}

		static private readonly TimeSpan BeforehandTime = new(0, 5, 0);
		static private readonly TimeSpan DeltaTime = TimeSpan.Zero;

		private readonly DataProvider _dataProvider;
		private readonly DataContext _dataContext;
		private readonly List<DailyNotification> _startSubjectDateTimeNotifications;
		private readonly List<DailyNotification> _endSubjectDateTimeNotifications;
		private readonly TelegramMessageSender _telegramMessageSender;
		private DateTime _startToday;

		public ScheduleNotifier(DataProvider dataProvider, DataContext dataContext, TelegramMessageSender telegramMessageSender)
		{
			_dataProvider = dataProvider ?? throw new ArgumentNullException(nameof(dataProvider));
			_dataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));
			_telegramMessageSender = telegramMessageSender ?? throw new ArgumentNullException(nameof(telegramMessageSender));

			_startSubjectDateTimeNotifications = new List<DailyNotification>();
			_endSubjectDateTimeNotifications = new List<DailyNotification>();
		}

		public void Notify()
		{
			if (_dataProvider.MoscowDateTime.Day != _startToday.Day)
				UpdateDailyNotifications();

			NotifyTimeSlotBound(_startSubjectDateTimeNotifications, NotifyFirstSubjectStart);
			NotifyTimeSlotBound(_endSubjectDateTimeNotifications, NotifySubjectEnd);
		}

		public async void NotifyFirstSubjectStart(int timeSlotOrder)
		{
			foreach (Group group in _dataContext.Groups)
			{
				List<ScheduleView> schedule = ScheduleViewReader.GetSchedule(group, _dataProvider.MoscowDateTime.DayOfWeek, group.Parity(_dataProvider.MoscowDateTime));
				if (schedule == null || schedule.Count == 0)
					continue;
				int firstSubjectTimeSlotOrder = schedule.Select(x => x.Order).Min();
				if (firstSubjectTimeSlotOrder != timeSlotOrder)
					continue;
				ScheduleView subject = schedule.FirstOrDefault(x => x.Order == timeSlotOrder);
				if (subject != null)
				{
					foreach (Student user in group.Students)
						await _telegramMessageSender.NotifyFirstSubjectStart(user.ChatId, subject.SubjectInstance);
				}
			}
		}
		public async void NotifySubjectEnd(int order)
		{
			foreach (Group group in _dataContext.Groups)
			{
				List<ScheduleView> schedule = ScheduleViewReader.GetSchedule(group, _dataProvider.MoscowDateTime.DayOfWeek, group.Parity(_dataProvider.MoscowDateTime));
				if (schedule == null || schedule.Count == 0)
					continue;
				ScheduleView subject = schedule.FirstOrDefault(x => x.Order == order);
				if (subject != null)
				{
					foreach (Student user in group.Students)
					{
						ScheduleView nextSubject = schedule.FirstOrDefault(z => z.Order == order + 1);
						if (nextSubject != null)
							await _telegramMessageSender.NotifySubjectEnd(user.ChatId, subject.SubjectInstance, nextSubject.SubjectInstance);
						else
							await _telegramMessageSender.NotifySubjectEnd(user.ChatId, subject.SubjectInstance);
					}
				}
			}
		}

		private delegate void NotifySubjectTimeSlot(int timeSlotOrder);
		private void NotifyTimeSlotBound(IEnumerable<DailyNotification> notifications, NotifySubjectTimeSlot notify)
		{
			foreach (DailyNotification notificationData in notifications)
			{
				if (notificationData.IsNotified)
					continue;

				if (_dataProvider.MoscowDateTime < notificationData.DateTime)
					continue;

				notify(notificationData.TimeSlotOrder);
				notificationData.IsNotified = true;
			}
		}

		private void UpdateDailyNotifications()
		{
			_startToday = _dataProvider.MoscowDateTime.Date;
			_startSubjectDateTimeNotifications.Clear();
			_endSubjectDateTimeNotifications.Clear();
			List<SubjectTimeSlot> subjectTimeSlots = new(_dataContext.SubjectTimeSlots);

			foreach (SubjectTimeSlot timeSlot in subjectTimeSlots)
			{
				DateTime startLessonTime = _startToday.Add(timeSlot.Start - BeforehandTime);
				DateTime endLessonTime = _startToday.Add(timeSlot.End);

				if ((_dataProvider.MoscowDateTime - startLessonTime) < DeltaTime)
					_startSubjectDateTimeNotifications.Add(new DailyNotification(startLessonTime, timeSlot.Order));
				if ((_dataProvider.MoscowDateTime - endLessonTime) < DeltaTime)
					_endSubjectDateTimeNotifications.Add(new DailyNotification(endLessonTime, timeSlot.Order));
			}
		}
	}
}
