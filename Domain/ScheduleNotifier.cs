using Database.Data;
using Database.Data.Model;
using Domain.Data;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

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
		private readonly ILogger<ScheduleNotifier> _logger;
		private readonly List<DailyNotification> _startSubjectDateTimeNotifications;
		private readonly List<DailyNotification> _endSubjectDateTimeNotifications;
		private DateTime _startToday;

		public ScheduleNotifier(DataProvider dataProvider, DataContext dataContext, ILogger<ScheduleNotifier> logger)
		{
			_dataProvider = dataProvider ?? throw new ArgumentNullException(nameof(dataProvider));
			_dataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));

			_startSubjectDateTimeNotifications = new List<DailyNotification>();
			_endSubjectDateTimeNotifications = new List<DailyNotification>();
		}

		public void Notify()
		{
			if (_dataProvider.MoscowDateTime.Day != _startToday.Day)
				UpdateDailyNotifications();

			NotifyTimeSlotBound(_startSubjectDateTimeNotifications, _dataProvider.NotifyFirstSubjectStart);
			NotifyTimeSlotBound(_endSubjectDateTimeNotifications, _dataProvider.NotifySubjectEnd);
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
			_logger.LogInformation($"" +
				$"Invoked {nameof(UpdateDailyNotifications)}. " +
				$"{nameof(_dataProvider.MoscowDateTime)}: {_dataProvider.MoscowDateTime}. " +
				$"{nameof(_startToday)}: {_startToday}. "
			);
			_startToday = _dataProvider.MoscowDateTime.Date;
			_startSubjectDateTimeNotifications.Clear();
			_endSubjectDateTimeNotifications.Clear();
			List<SubjectTimeSlot> subjectTimeSlots = new(_dataContext.SubjectTimeSlots);

			foreach (SubjectTimeSlot timeSlot in subjectTimeSlots)
			{
				DateTime startLessonTime = _startToday.Add(timeSlot.Start - BeforehandTime);
				DateTime endLessonTime = _startToday.Add(timeSlot.End);

				if ((_dataProvider.MoscowDateTime - startLessonTime) < DeltaTime)
				{
					_startSubjectDateTimeNotifications.Add(new DailyNotification(startLessonTime, timeSlot.Order));
					_logger.LogInformation($"Added daily start timeSlot notification at {startLessonTime}.");
				}
				if ((_dataProvider.MoscowDateTime - endLessonTime) < DeltaTime)
				{
					_endSubjectDateTimeNotifications.Add(new DailyNotification(endLessonTime, timeSlot.Order));
					_logger.LogInformation($"Added daily end timeSlot notification at {endLessonTime}.");
				}
			}
		}
	}
}
